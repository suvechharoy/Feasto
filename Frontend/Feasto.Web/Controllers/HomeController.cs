using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Mvc;
using Feasto.Web.Models;
using Feasto.Web.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;

namespace Feasto.Web.Controllers;

public class HomeController : Controller
{
    private readonly IProductService _productService;
    private readonly ICartService _cartService;
    public HomeController(IProductService productService, ICartService cartService)
    {
        _productService = productService;
        _cartService = cartService; 
    }

    public async Task<IActionResult> Index()
    {
        List<ProductDTO> list = new List<ProductDTO>();
        ResponseDTO? response = await _productService.GetAllProductsAsync();
        if (response != null && response.IsSuccess)
        {
            list = JsonConvert.DeserializeObject<List<ProductDTO>>(Convert.ToString(response.Result));
        }
        else
        {
            TempData["error"] = response?.Message;
        }
        return View(list);
    }
    [Authorize]
    public async Task<IActionResult> ProductDetails(int productId)
    {
        ProductDTO? model = new ProductDTO();
        ResponseDTO? response = await _productService.GetProductByIdAsync(productId);
        if (response != null && response.IsSuccess)
        {
            model = JsonConvert.DeserializeObject<ProductDTO>(Convert.ToString(response.Result));
        }
        else
        {
            TempData["error"] = response?.Message;
        }
        return View(model);
    }

    [HttpPost]
    [Authorize]
    [ActionName("ProductDetails")]
    public async Task<IActionResult> ProductDetails(ProductDTO productDto)
    {
        CartDTO cartDto = new CartDTO()
        {
            CartHeader = new CartHeaderDTO()
            {
                UserId = User.Claims.Where(u=>u.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value,
            }
        };

        CartDetailsDTO cartDetailsDto = new CartDetailsDTO()
        {
            Count = productDto.Count,
            ProductId = productDto.ProductId,
        };
        
        List<CartDetailsDTO> cartDetails = new() { cartDetailsDto };
        cartDto.CartDetails = cartDetails;
        
        ResponseDTO? response = await _cartService.UpsertCartAsync(cartDto);
        if (response != null && response.IsSuccess)
        {
            TempData["success"] = "Item added to cart!";
            return RedirectToAction("Index");
        }
        else
        {
            TempData["error"] = response?.Message;
        }
        return View(productDto);
    }
    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}