using Feasto.Web.Models;

namespace Feasto.Web.Service.IService;

public interface ICartService
{
    Task<ResponseDTO?> GetCartByUserIdAsync(string userId);
    Task<ResponseDTO?> UpsertCartAsync(CartDTO cartDto);
    Task<ResponseDTO?> RemoveFromCartAsync(int cartDetailsId);
    Task<ResponseDTO?> ApplyCouponAsync(CartDTO cartDto);
    Task<ResponseDTO?> EmailCart(CartDTO cartDto);
}
    