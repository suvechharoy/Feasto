using AutoMapper;
using Feasto.Services.OrderAPI.Data;
using Feasto.Services.OrderAPI.Models;
using Feasto.Services.OrderAPI.Models.DTO;
using Feasto.Services.OrderAPI.Service.IService;
using Feasto.Services.OrderAPI.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;

namespace Feasto.Services.OrderAPI.Controllers
{
    [Route("api/order")]
    [ApiController]
    public class OrderAPIController : ControllerBase
    {
        protected ResponseDTO _response;
        private IMapper _mapper;
        private readonly AppDbContext _context;
        private IProductService _productService;
        
        public OrderAPIController(AppDbContext context, IMapper mapper, IProductService productService)
        {
            _context = context;
            _mapper = mapper;
            _productService = productService;
            this._response = new ResponseDTO();
        }

        [Authorize]
        [HttpPost("CreateOrder")]
        public async Task<ResponseDTO> CreateOrder([FromBody] CartDTO cartDTO)
        {
            try
            {
                OrderHeaderDTO orderHeaderDTO = _mapper.Map<OrderHeaderDTO>(cartDTO.CartHeader);
                orderHeaderDTO.OrderTime = DateTime.UtcNow;
                orderHeaderDTO.Status = StaticDetails.Status_Pending;
                orderHeaderDTO.OrderDetails = _mapper.Map<IEnumerable<OrderDetailsDTO>>(cartDTO.CartDetails);

                OrderHeader orderCreated = 
                    _context.OrderHeaders.Add(_mapper.Map<OrderHeader>(orderHeaderDTO)).Entity;
                await _context.SaveChangesAsync();
                
                orderHeaderDTO.OrderHeaderId = orderCreated.OrderHeaderId;
                _response.Result = orderHeaderDTO;
            }
            catch (Exception e)
            {
                _response.IsSuccess = false;
                _response.Message = e.Message;
            }
            return _response;
        }

        [Authorize]
        [HttpPost("CreateStripeSession")]
        public async Task<ResponseDTO> CreateStripeSession([FromBody] StripeRequestDTO stripeRequestDTO)
        {
            try
            {
                var options = new SessionCreateOptions
                {
                    SuccessUrl = stripeRequestDTO.ApprovedUrl,
                    CancelUrl = stripeRequestDTO.CancelUrl,
                    LineItems = new List<SessionLineItemOptions>(),
                    Mode = "payment",
                };

                var discountObj = new List<SessionDiscountOptions>()
                {
                    new SessionDiscountOptions
                    {
                        Coupon = stripeRequestDTO.OrderHeader.CouponCode
                    }
                };

                foreach (var item in stripeRequestDTO.OrderHeader.OrderDetails)
                {
                    var sessionLineItem = new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions()
                        {
                            UnitAmount = (long)(item.Price * 100), //$20.99 -> 2099
                            Currency = "usd",
                            ProductData = new SessionLineItemPriceDataProductDataOptions()
                            {
                                Name = item.ProductName,
                            }
                        },
                        Quantity = item.Count
                    };
                    options.LineItems.Add(sessionLineItem);
                }

                if (stripeRequestDTO.OrderHeader.Discount > 0)
                {
                    options.Discounts = discountObj;
                }
                
                var service = new SessionService();
                Session session = service.Create(options);

                stripeRequestDTO.StripeSessionUrl = session.Url;
                OrderHeader orderHeader =
                    _context.OrderHeaders.First(u => u.OrderHeaderId == stripeRequestDTO.OrderHeader.OrderHeaderId);
                orderHeader.StripeSessionId = session.Id;
                _context.SaveChanges();
                _response.Result = stripeRequestDTO;
            }
            catch (Exception e)
            {
                _response.IsSuccess = false;
                _response.Message = e.Message;
            }
            return _response;
        }
        
        [Authorize]
        [HttpPost("ValidateStripeSession")]
        public async Task<ResponseDTO> ValidateStripeSession([FromBody] int orderHeaderId)
        {
            try
            {
                OrderHeader orderHeader = _context.OrderHeaders.First(u => u.OrderHeaderId == orderHeaderId);

                var service = new SessionService();
                Session session = service.Get(orderHeader.StripeSessionId);

                var paymentIntentService = new PaymentIntentService();
                PaymentIntent paymentIntent = paymentIntentService.Get(session.PaymentIntentId);

                if (paymentIntent.Status == "succeeded")
                {
                    //if payment was successful
                    orderHeader.PaymentIntentId = paymentIntent.Id;
                    orderHeader.Status = StaticDetails.Status_Approved;
                    _context.SaveChanges();

                    _response.Result = _mapper.Map<OrderHeaderDTO>(orderHeader);
                }
            }
            catch (Exception e)
            {
                _response.IsSuccess = false;
                _response.Message = e.Message;
            }
            return _response;
        }
    }
}
