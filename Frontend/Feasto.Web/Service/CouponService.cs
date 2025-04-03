using Feasto.Web.Models;
using Feasto.Web.Service.IService;
using Feasto.Web.Utility;

namespace Feasto.Web.Service;

public class CouponService : ICouponService
{
    private readonly IBaseService _baseService;

    public CouponService(IBaseService baseService)
    {
        _baseService = baseService;
    }
    
    public async Task<ResponseDTO?> GetCouponAsync(string couponCode)
    {
        return await _baseService.SendAsync(new RequestDTO()
        {
            ApiType = StaticDetails.ApiType.GET,
            Url = StaticDetails.CouponAPIBase + "/api/coupon/GetByCode/"+couponCode,
        });
    }

    public async Task<ResponseDTO?> GetAllCouponsAsync()
    {
        return await _baseService.SendAsync(new RequestDTO()
        {
            ApiType = StaticDetails.ApiType.GET,
            Url = StaticDetails.CouponAPIBase + "/api/coupon"
        });
    }

    public async Task<ResponseDTO?> GetCouponByIdAsync(int id)
    {
        return await _baseService.SendAsync(new RequestDTO()
        {
            ApiType = StaticDetails.ApiType.GET,
            Url = StaticDetails.CouponAPIBase + "/api/coupon/"+id
        });
    }

    public async Task<ResponseDTO?> CreateCouponsAsync(CouponDTO couponDto)
    {
        return await _baseService.SendAsync(new RequestDTO()
        {
            ApiType = StaticDetails.ApiType.POST,
            Data = couponDto,
            Url = StaticDetails.CouponAPIBase + "/api/coupon"
        });
    }

    public async Task<ResponseDTO?> UpdateCouponsAsync(CouponDTO couponDto)
    {
        return await _baseService.SendAsync(new RequestDTO()
        {
            ApiType = StaticDetails.ApiType.PUT,
            Data = couponDto,
            Url = StaticDetails.CouponAPIBase + "/api/coupon"
        });
    }

    public async Task<ResponseDTO?> DeleteCouponsAsync(int id)
    {
        return await _baseService.SendAsync(new RequestDTO()
        {
            ApiType = StaticDetails.ApiType.DELETE,
            Url = StaticDetails.CouponAPIBase + "/api/coupon/"+id
        });
    }
}