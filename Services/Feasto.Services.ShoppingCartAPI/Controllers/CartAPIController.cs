using AutoMapper;
using Feasto.MessageBus;
using Feasto.Services.ShoppingCartAPI.Data;
using Feasto.Services.ShoppingCartAPI.Models;
using Feasto.Services.ShoppingCartAPI.Models.DTO;
using Feasto.Services.ShoppingCartAPI.RabbitMQSender;
using Feasto.Services.ShoppingCartAPI.Service.IService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Feasto.Services.ShoppingCartAPI.Controllers
{
    [Route("api/cart")]
    [ApiController]
    public class CartAPIController : ControllerBase
    {
        private readonly AppDbContext _db;
        private IMapper _mapper;
        private ResponseDTO _response;
        private readonly IProductService _productService;
        private readonly ICouponService _couponService;
        private readonly IRabbitMQCartMessageSender _messageBus;
        private IConfiguration _configuration;

        public CartAPIController(AppDbContext db, IMapper mapper, IProductService productService, ICouponService couponService, IRabbitMQCartMessageSender messageBus, IConfiguration configuration)
        {
            _db = db;
            _mapper = mapper;
            this._response = new ResponseDTO();
            _productService = productService;
            _couponService = couponService;
            _messageBus = messageBus;   
            _configuration = configuration;
        }

        [HttpGet("GetCart/{userId}")]
        public async Task<ResponseDTO> GetCart(string userId)
        {
            try
            {
                CartDTO cart = new CartDTO()
                {
                    CartHeader = _mapper.Map<CartHeaderDTO>(_db.CartHeaders.First(u=>u.UserId == userId)),
                };
                cart.CartDetails =
                    _mapper.Map<IEnumerable<CartDetailsDTO>>(_db.CartDetails.Where(u =>
                        u.CartHeaderId == cart.CartHeader.CartHeaderId));
                
                IEnumerable<ProductDTO> products = await _productService.GetProducts();
                
                foreach (var item in cart.CartDetails)
                {
                    item.Product = products.FirstOrDefault(p => p.ProductId == item.ProductId);  
                    cart.CartHeader.CartTotal += item.Count * item.Product.Price;
                }
                // Apply coupon if any
                if (!string.IsNullOrEmpty(cart.CartHeader.CouponCode))
                {
                    CouponDTO coupon = await _couponService.GetCoupon(cart.CartHeader.CouponCode);
                    if(coupon != null && cart.CartHeader.CartTotal > coupon.MinAmount)
                    {
                        cart.CartHeader.CartTotal -= coupon.DiscountAmount;
                        cart.CartHeader.Discount = coupon.DiscountAmount;
                    }
                }

                _response.Result = cart;
            }
            catch (Exception e)
            {
                _response.Message = e.Message;
                _response.IsSuccess = false;
            }
            return _response;
        }

        [HttpPost("ApplyCoupon")]
        public async Task<object> ApplyCoupon([FromBody] CartDTO cartDto)
        {
            try
            {
                var cartFromDb = await _db.CartHeaders.FirstAsync(u => u.UserId == cartDto.CartHeader.UserId); // In order to apply a coupon, first fetch the cart details from db
                cartFromDb.CouponCode = cartDto.CartHeader.CouponCode;
                _db.CartHeaders.Update(cartFromDb);
                await _db.SaveChangesAsync();
                _response.Result = true;
            }
            catch (Exception e)
            {
                _response.Message = e.Message.ToString();
                _response.IsSuccess = false;
            }
            return _response;
        }
        
        [HttpPost("EmailCartRequest")]
        public async Task<object> EmailCartRequest([FromBody] CartDTO cartDto)
        {
            try
            {
                _messageBus.SendMessage(cartDto, _configuration.GetValue<string>("TopicAndQueueNames:EmailShoppingCartQueue"));
                _response.Result = true;
            }
            catch (Exception e)
            {
                _response.Message = e.Message.ToString();
                _response.IsSuccess = false;
            }
            return _response;
        }
        
        [HttpPost("CartUpsert")]
        public async Task<ResponseDTO> Upsert(CartDTO cartDTO)
        {
            try
            {
                var cartHeaderFromDb = await _db.CartHeaders.AsNoTracking().FirstOrDefaultAsync(u => u.UserId == cartDTO.CartHeader.UserId); //Fetch cart header from db.
                if (cartHeaderFromDb == null) // If there is no cart header i.e., user is placing order for first time.
                {
                    // Create Cart Header & Details
                    CartHeader cartHeader = _mapper.Map<CartHeader>(cartDTO.CartHeader);
                    _db.CartHeaders.Add(cartHeader);
                    await _db.SaveChangesAsync();
                    cartDTO.CartDetails.First().CartHeaderId = cartHeader.CartHeaderId;
                    _db.CartDetails.Add(_mapper.Map<CartDetails>(cartDTO.CartDetails.First()));
                    await _db.SaveChangesAsync();
                }
                else
                {
                    // If Cart Header is NOT NULL
                    // Check if Cart Details has same product
                    var cartDetailsFromDb = await _db.CartDetails.AsNoTracking().FirstOrDefaultAsync(
                        u => u.ProductId == cartDTO.CartDetails.First().ProductId && 
                             u.CartHeaderId == cartHeaderFromDb.CartHeaderId); // Here we're checking if Cart Details has any product or not & also checking if header id is same or not, as it may be possible that another user might have the same product in their cart. 
                    if (cartDetailsFromDb == null) // If there are no products in the cart, create new cart details.
                    {
                        // Create Cart Details
                        cartDTO.CartDetails.First().CartHeaderId = cartHeaderFromDb.CartHeaderId;
                        _db.CartDetails.Add(_mapper.Map<CartDetails>(cartDTO.CartDetails.First()));
                        await _db.SaveChangesAsync();
                        
                    }
                    else // If they already have the product in their cart.
                    {
                        // Update count in Cart Details
                        cartDTO.CartDetails.First().Count += cartDetailsFromDb.Count;
                        cartDTO.CartDetails.First().CartHeaderId = cartDetailsFromDb.CartHeaderId;
                        cartDTO.CartDetails.First().CartDetailsId = cartDetailsFromDb.CartDetailsId;
                        _db.CartDetails.Update(_mapper.Map<CartDetails>(cartDTO.CartDetails.First()));
                        await _db.SaveChangesAsync();
                    }
                }

                _response.Result = cartDTO;
            }
            catch (Exception e)
            {
                _response.Message = e.Message.ToString();
                _response.IsSuccess = false;
            }
            return _response;
        }
        
        [HttpPost("RemoveCart")]
        public async Task<ResponseDTO> RemoveCart([FromBody] int cartDetailsId)
        {
            try
            {
                CartDetails cartDetails = _db.CartDetails.First(u => u.CartDetailsId == cartDetailsId);
                int totalCountOfCartItem =
                    _db.CartDetails.Where(u => u.CartHeaderId == cartDetails.CartHeaderId).Count(); // For any particular user cart header will be common and different products can be added under same cart header id, so we'll count products based on cart header id. 
                _db.CartDetails.Remove(cartDetails);
                if (totalCountOfCartItem == 1)
                {
                    var cartHeaderToRemove =
                        await _db.CartHeaders.FirstOrDefaultAsync(u => u.CartHeaderId == cartDetails.CartHeaderId);
                    _db.CartHeaders.Remove(cartHeaderToRemove);
                }
                await _db.SaveChangesAsync();
                _response.Result = true;
            }
            catch (Exception e)
            {
                _response.Message = e.Message.ToString();
                _response.IsSuccess = false;
            }
            return _response;
        }
    }
}
