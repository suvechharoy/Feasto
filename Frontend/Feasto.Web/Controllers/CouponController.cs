using Feasto.Web.Models;
using Feasto.Web.Service.IService;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Feasto.Web.Controllers;

public class CouponController : Controller
{
    private readonly ICouponService _couponService;

    public CouponController(ICouponService couponService)
    {
        _couponService = couponService;
    }

    public async Task<IActionResult> CouponIndex()
    {
        List<CouponDTO>? list = new List<CouponDTO>();
        ResponseDTO? response = await _couponService.GetAllCouponsAsync();
        if (response != null && response.IsSuccess)
        {
            list = JsonConvert.DeserializeObject<List<CouponDTO>>(Convert.ToString(response.Result));
        }
        else
        {
            TempData["error"] = response?.Message;
        }
        return View(list);
    }

    public async Task<IActionResult> CouponCreate()
    {
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> CouponCreate(CouponDTO model)
    {
        if (ModelState.IsValid)
        {
            ResponseDTO? response = await _couponService.CreateCouponsAsync(model);
            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Coupon created successfully!";
                return RedirectToAction("CouponIndex");
            }
            else
            {
                TempData["error"] = response?.Message;
            }
        }
        return View(model);
    }
    public async Task<IActionResult> CouponDelete(int couponId)
    {
        ResponseDTO? response = await _couponService.GetCouponByIdAsync(couponId);
        if (response != null && response.IsSuccess)
        {
            CouponDTO? model = JsonConvert.DeserializeObject<CouponDTO>(Convert.ToString(response.Result));
            return View(model);
        }
        else
        {
            TempData["error"] = response?.Message;
        }
        return NotFound();
    }

    [HttpPost]
    public async Task<IActionResult> CouponDelete(CouponDTO couponDto)
    {
        ResponseDTO? response = await _couponService.DeleteCouponsAsync(couponDto.CouponId);
        if (response != null && response.IsSuccess)
        {
            TempData["success"] = "Coupon deleted successfully!";
            return RedirectToAction("CouponIndex");
        }
        else
        {
            TempData["error"] = response?.Message;
        }
        return View(couponDto);
    }
}