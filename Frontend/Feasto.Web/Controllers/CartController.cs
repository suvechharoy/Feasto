using System.IdentityModel.Tokens.Jwt;
using Feasto.Web.Models;
using Feasto.Web.Service.IService;
using Feasto.Web.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Feasto.Web.Controllers
{
    public class CartController : Controller
    {
        private readonly ICartService _cartService;
        private readonly IOrderService _orderService;

        public CartController(ICartService cartService, IOrderService orderService)
        {
            _cartService = cartService;
            _orderService = orderService;
        }
        
        [Authorize]
        public async Task<IActionResult> CartIndex()
        {
            return View(await LoadCartBasedOnLoggedInUser());
        }
        
        [Authorize]
        public async Task<IActionResult> Checkout()
        {
            return View(await LoadCartBasedOnLoggedInUser());
        }

        [HttpPost]
        [ActionName("Checkout")]
        public async Task<IActionResult> Checkout(CartDTO cartDTO)
        {
            CartDTO cart = await LoadCartBasedOnLoggedInUser();
            cart.CartHeader.FirstName = cartDTO.CartHeader.FirstName;
            cart.CartHeader.LastName = cartDTO.CartHeader.LastName;
            cart.CartHeader.Email = cartDTO.CartHeader.Email;
            cart.CartHeader.Phone = cartDTO.CartHeader.Phone;
            
            var response = await _orderService.CreateOrder(cart);
                OrderHeaderDTO orderHeaderDto = JsonConvert.DeserializeObject<OrderHeaderDTO>(Convert.ToString(response.Result));

            if (response != null && response.IsSuccess)
            {
                //get stripe session and redirect to stripe to place order
                var domain = Request.Scheme + "://" + Request.Host.Value + "/";

                StripeRequestDTO stripeRequestDto = new()
                {
                    ApprovedUrl = domain + "cart/Confirmation?orderId=" + orderHeaderDto.OrderHeaderId,
                    CancelUrl = domain + "cart/Checkout",
                    OrderHeader = orderHeaderDto
                };
                var stripeResponse = await _orderService.CreateStripeSession(stripeRequestDto);
                StripeRequestDTO stripeResponseResult = JsonConvert.DeserializeObject<StripeRequestDTO>(Convert.ToString(stripeResponse.Result));
                Response.Headers.Add("Location", stripeResponseResult.StripeSessionUrl); //redirect to stripe session
                return new StatusCodeResult(303); //status code for redirection to another page
            }
            return View();
        }
        
        [Authorize]
        public async Task<IActionResult> Confirmation(int orderId)
        {
            ResponseDTO? response = await _orderService.ValidateStripeSession(orderId);
            if (response != null && response.IsSuccess)
            {
                OrderHeaderDTO orderHeader = JsonConvert.DeserializeObject<OrderHeaderDTO>(Convert.ToString(response.Result));
                if (orderHeader.Status == StaticDetails.Status_Approved)
                {
                    return View(orderId);
                }
            }   
            //either return back to view or redirect to some other page based on status
            return View(orderId);
        }

        public async Task<IActionResult> Remove(int cartDetailsId)
        {
            var userId = User.Claims.Where(u=> u.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;
            ResponseDTO? response = await _cartService.RemoveFromCartAsync(cartDetailsId);
            if (response != null && response.IsSuccess)
            {
                TempData["Success"] = "Cart updated successfully!";
                return RedirectToAction(nameof(CartIndex));
            }
            return View();
        }
        
        [HttpPost]
        public async Task<IActionResult> ApplyCoupon(CartDTO cartDTO)
        {
            ResponseDTO? response = await _cartService.ApplyCouponAsync(cartDTO);
            if (response != null && response.IsSuccess)
            {
                TempData["Success"] = "Cart updated successfully!";
                return RedirectToAction(nameof(CartIndex));
            }
            return View();
        }
        
        [HttpPost]
        public async Task<IActionResult> EmailCart(CartDTO cartDTO)
        {
            CartDTO cart = await LoadCartBasedOnLoggedInUser();
            cart.CartHeader.Email = User.Claims.Where(u=> u.Type == JwtRegisteredClaimNames.Email)?.FirstOrDefault()?.Value;

            ResponseDTO? response = await _cartService.EmailCart(cart);
            if (response != null && response.IsSuccess)
            {
                TempData["Success"] = "Email will be processed and sent shortly.";
                return RedirectToAction(nameof(CartIndex));
            }
            return View();
        }
        
        [HttpPost]
        public async Task<IActionResult> RemoveCoupon(CartDTO cartDTO)
        {
            cartDTO.CartHeader.CouponCode = "";
            ResponseDTO? response = await _cartService.ApplyCouponAsync(cartDTO);
            if (response != null && response.IsSuccess)
            {
                TempData["Success"] = "Cart updated successfully!";
                return RedirectToAction(nameof(CartIndex));
            }
            return View();
        }
        private async Task<CartDTO> LoadCartBasedOnLoggedInUser()
        {
            var userId = User.Claims.Where(u=> u.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;
            ResponseDTO? response = await _cartService.GetCartByUserIdAsync(userId);
            if (response != null && response.IsSuccess)
            {
                CartDTO cartDTO = JsonConvert.DeserializeObject<CartDTO>(Convert.ToString(response.Result));
                return cartDTO;
            }
            return new CartDTO();
        }
    }
}
