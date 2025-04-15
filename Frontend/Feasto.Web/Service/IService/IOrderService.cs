using Feasto.Web.Models;

namespace Feasto.Web.Service.IService;

public interface IOrderService
{
    Task<ResponseDTO?> CreateOrder(CartDTO cartDTO);
    Task<ResponseDTO?> CreateStripeSession(StripeRequestDTO stripeRequestDTO);
    Task<ResponseDTO?> ValidateStripeSession(int orderHeaderId);
}