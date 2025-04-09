using Feasto.Services.ShoppingCartAPI.Models.DTO;
using Feasto.Services.ShoppingCartAPI.Service.IService;
using Newtonsoft.Json;

namespace Feasto.Services.ShoppingCartAPI.Service;

public class CouponService : ICouponService
{
    private readonly IHttpClientFactory _httpClientFactory;

    public CouponService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }
    
    public async Task<CouponDTO> GetCoupon(string couponCode)
    {
        var client = _httpClientFactory.CreateClient("Coupon");
        var result = await client.GetAsync($"/api/coupon/GetByCode/{couponCode}");
        var apiContent = await result.Content.ReadAsStringAsync();
        var response = JsonConvert.DeserializeObject<ResponseDTO>(apiContent);
        if (response.IsSuccess)
        {
            return JsonConvert.DeserializeObject<CouponDTO>(Convert.ToString(response.Result));
        }
        return new CouponDTO();
    }
}