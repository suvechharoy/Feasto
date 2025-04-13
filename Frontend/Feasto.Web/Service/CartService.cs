using Feasto.Web.Models;
using Feasto.Web.Service.IService;
using Feasto.Web.Utility;

namespace Feasto.Web.Service;

public class CartService : ICartService
{
    private readonly IBaseService _baseService;

    public CartService(IBaseService baseService)
    {
        _baseService = baseService;
    }
    public async Task<ResponseDTO?> GetCartByUserIdAsync(string userId)
    {
        return await _baseService.SendAsync(new RequestDTO()
        {
            ApiType = StaticDetails.ApiType.GET,
            Url = StaticDetails.ShoppingCartAPIBase + "/api/cart/GetCart/"+userId
        });
    }

    public async Task<ResponseDTO?> UpsertCartAsync(CartDTO cartDto)
    {
        return await _baseService.SendAsync(new RequestDTO()
        {
            ApiType = StaticDetails.ApiType.POST,
            Data = cartDto,
            Url = StaticDetails.ShoppingCartAPIBase + "/api/cart/CartUpsert"
        });
    }

    public async Task<ResponseDTO?> RemoveFromCartAsync(int cartDetailsId)
    {
        return await _baseService.SendAsync(new RequestDTO()
        {
            ApiType = StaticDetails.ApiType.POST,
            Data = cartDetailsId,
            Url = StaticDetails.ShoppingCartAPIBase + "/api/cart/RemoveCart"
        });
    }

    public async Task<ResponseDTO?> ApplyCouponAsync(CartDTO cartDto)
    {
        return await _baseService.SendAsync(new RequestDTO()
        {
            ApiType = StaticDetails.ApiType.POST,
            Data = cartDto,
            Url = StaticDetails.ShoppingCartAPIBase + "/api/cart/ApplyCoupon"
        });
    }

    public async Task<ResponseDTO?> EmailCart(CartDTO cartDto)
    {
        return await _baseService.SendAsync(new RequestDTO()
        {
            ApiType = StaticDetails.ApiType.POST,
            Data = cartDto,
            Url = StaticDetails.ShoppingCartAPIBase + "/api/cart/EmailCartRequest"
        });
    }
}