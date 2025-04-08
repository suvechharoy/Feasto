using Feasto.Web.Models;
using Feasto.Web.Service.IService;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Feasto.Web.Controllers;

public class ProductController : Controller
{
    private readonly IProductService _productService;

    public ProductController(IProductService productService)
    {
        _productService = productService;
    }

    public async Task<IActionResult> ProductIndex()
    {
        List<ProductDTO>? list = new List<ProductDTO>();
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

    public async Task<IActionResult> ProductCreate()
    {
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> ProductCreate(ProductDTO model)
    {
        if (ModelState.IsValid)
        {
            ResponseDTO? response = await _productService.CreateProductsAsync(model);
            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Product created successfully!";
                return RedirectToAction("ProductIndex");
            }
            else
            {
                TempData["error"] = response?.Message;
            }
        }
        return View(model);
    }
    public async Task<IActionResult> ProductDelete(int productId)
    {
        ResponseDTO? response = await _productService.GetProductByIdAsync(productId);
        if (response != null && response.IsSuccess)
        {
            ProductDTO? model = JsonConvert.DeserializeObject<ProductDTO>(Convert.ToString(response.Result));
            return View(model);
        }
        else
        {
            TempData["error"] = response?.Message;
        }
        return NotFound();
    }

    [HttpPost]
    public async Task<IActionResult> ProductDelete(ProductDTO productDto)
    {
        ResponseDTO? response = await _productService.DeleteProductsAsync(productDto.ProductId);
        if (response != null && response.IsSuccess)
        {
            TempData["success"] = "Product deleted successfully!";
            return RedirectToAction("ProductIndex");
        }
        else
        {
            TempData["error"] = response?.Message;
        }
        return View(productDto);
    }
    public async Task<IActionResult> ProductEdit(int productId)
    {
        ResponseDTO? response = await _productService.GetProductByIdAsync(productId);
        if (response != null && response.IsSuccess)
        {
            ProductDTO? model = JsonConvert.DeserializeObject<ProductDTO>(Convert.ToString(response.Result));
            return View(model);
        }
        else
        {
            TempData["error"] = response?.Message;
        }
        return NotFound();
    }

    [HttpPost]
    public async Task<IActionResult> ProductEdit(ProductDTO productDto)
    {
        ResponseDTO? response = await _productService.UpdateProductsAsync(productDto);
        if (response != null && response.IsSuccess)
        {
            TempData["success"] = "Product updated successfully!";
            return RedirectToAction("ProductIndex");
        }
        else
        {
            TempData["error"] = response?.Message;
        }
        return View(productDto);
    }
}