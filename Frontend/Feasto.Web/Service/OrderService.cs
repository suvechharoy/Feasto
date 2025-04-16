using Feasto.Web.Models;
using Feasto.Web.Service.IService;
using Feasto.Web.Utility;

namespace Feasto.Web.Service;

public class OrderService : IOrderService
{
    private readonly IBaseService _baseService;

    public OrderService(IBaseService baseService)
    {
        _baseService = baseService;
    }

    public async Task<ResponseDTO?> CreateOrder(CartDTO cartDTO)
    {
        return await _baseService.SendAsync(new RequestDTO()
        {
            ApiType = StaticDetails.ApiType.POST,
            Data = cartDTO,
            Url = StaticDetails.OrderAPIBase + "/api/order/CreateOrder"
        });
    }

    public async Task<ResponseDTO?> CreateStripeSession(StripeRequestDTO stripeRequestDTO)
    {
        return await _baseService.SendAsync(new RequestDTO()
        {
            ApiType = StaticDetails.ApiType.POST,
            Data = stripeRequestDTO,
            Url = StaticDetails.OrderAPIBase + "/api/order/CreateStripeSession"
        });
    }

    public async Task<ResponseDTO?> ValidateStripeSession(int orderHeaderId)
    {
        return await _baseService.SendAsync(new RequestDTO()
        {
            ApiType = StaticDetails.ApiType.POST,
            Data = orderHeaderId,
            Url = StaticDetails.OrderAPIBase + "/api/order/ValidateStripeSession"
        });
    }

    public async Task<ResponseDTO?> GetAllOrders(string? userId)
    {
        return await _baseService.SendAsync(new RequestDTO()
        {
            ApiType = StaticDetails.ApiType.GET,
            Url = StaticDetails.OrderAPIBase + "/api/order/GetOrders/"+userId
        });
    }

    public async Task<ResponseDTO?> GetOrder(int orderId)
    {
        return await _baseService.SendAsync(new RequestDTO()
        {
            ApiType = StaticDetails.ApiType.GET,
            Url = StaticDetails.OrderAPIBase + "/api/order/GetOrder/"+orderId
        });
    }

    public async Task<ResponseDTO?> UpdateOrderStatus(int orderId, string newStatus)
    {
        return await _baseService.SendAsync(new RequestDTO()
        {
            ApiType = StaticDetails.ApiType.POST,
            Data = newStatus,
            Url = StaticDetails.OrderAPIBase + "/api/order/UpdateOrderStatus/"+orderId
        });
    }
}