using System.IdentityModel.Tokens.Jwt;
using Feasto.Web.Models;
using Feasto.Web.Service.IService;
using Feasto.Web.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Feasto.Web.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }
        
        [Authorize]
        public ActionResult OrderIndex()
        {
            return View();
        }
        
        [Authorize]
        public async Task<IActionResult> OrderDetail(int orderId)
        {
            OrderHeaderDTO orderHeaderDto = new OrderHeaderDTO();
            var userId = User.Claims.Where(c => c.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;
            var response = await _orderService.GetOrder(orderId);
            if (response != null && response.IsSuccess)
            {
                orderHeaderDto = JsonConvert.DeserializeObject<OrderHeaderDTO>(Convert.ToString(response.Result));
            }

            if (!User.IsInRole(StaticDetails.RoleAdmin) && userId != orderHeaderDto.UserId)
            {
                return NotFound();
            }

            return View(orderHeaderDto);
        }

        [HttpPost("OrderReadyForPickup")]
        public async Task<IActionResult> OrderReadyForPickup(int orderId)
        {
            var response = await _orderService.UpdateOrderStatus(orderId, StaticDetails.Status_ReadyForPickup);
            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Status updated successfully.";
                return RedirectToAction(nameof(OrderDetail), new { orderId = orderId });
            }
            return View();
        }
        
        [HttpPost("CompleteOrder")]
        public async Task<IActionResult> CompleteOrder(int orderId)
        {
            var response = await _orderService.UpdateOrderStatus(orderId, StaticDetails.Status_Completed);
            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Status updated successfully.";
                return RedirectToAction(nameof(OrderDetail), new { orderId = orderId });
            }
            return View();
        }
        
        [HttpPost("CancelOrder")]
        public async Task<IActionResult> CancelOrder(int orderId)
        {
            var response = await _orderService.UpdateOrderStatus(orderId, StaticDetails.Status_Cancelled);
            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Status updated successfully.";
                return RedirectToAction(nameof(OrderDetail), new { orderId = orderId });
            }
            return View();
        }

        [HttpGet]
        public IActionResult GetAll(string status)
        {
            IEnumerable<OrderHeaderDTO> list;
            string userId = "";
            if (!User.IsInRole(StaticDetails.RoleAdmin))
            {
                userId = User.Claims.Where(c=>c.Type==JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;
            }

            ResponseDTO response = _orderService.GetAllOrders(userId).GetAwaiter().GetResult();
            if (response != null && response.IsSuccess)
            {
                list = JsonConvert.DeserializeObject<List<OrderHeaderDTO>>(Convert.ToString(response.Result));
                switch (status)
                {
                    case "approved":
                        list = list.Where(c => c.Status == StaticDetails.Status_Approved);
                        break;
                    case "readyforpickup":
                        list = list.Where(c => c.Status == StaticDetails.Status_ReadyForPickup);
                        break;
                    case "cancelled":
                        list = list.Where(c => c.Status == StaticDetails.Status_Cancelled || c.Status == StaticDetails.Status_Refunded);
                        break;
                    default:
                        break;
                }
            }
            else
            {
                list = new List<OrderHeaderDTO>();
            }
            return Json(new{data=list});
        }
    }
}
