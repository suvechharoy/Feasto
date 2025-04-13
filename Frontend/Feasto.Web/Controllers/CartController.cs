using System.IdentityModel.Tokens.Jwt;
using Feasto.Web.Models;
using Feasto.Web.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Feasto.Web.Controllers
{
    public class CartController : Controller
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }
        
        [Authorize]
        public async Task<IActionResult> CartIndex()
        {
            return View(await LoadCartBasedOnLoggedInUser());
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
