using Feasto.Services.ShoppingCartAPI.Models.DTO;

namespace Feasto.Services.ShoppingCartAPI.Service.IService;

public interface ICouponService
{
    Task<CouponDTO> GetCoupon(string couponCode);
}