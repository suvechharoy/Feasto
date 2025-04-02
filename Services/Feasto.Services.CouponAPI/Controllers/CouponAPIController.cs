using Microsoft.AspNetCore.Mvc;

namespace Feasto.Services.CouponAPI.Controllers;

public class CouponAPIController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}