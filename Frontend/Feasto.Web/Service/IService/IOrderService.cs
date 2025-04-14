using Feasto.Web.Models;

namespace Feasto.Web.Service.IService;

public interface IOrderService
{
    Task<ResponseDTO?> CreateOrder(CartDTO cartDTO);
}