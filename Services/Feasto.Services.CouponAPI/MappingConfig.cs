using AutoMapper;
using Feasto.Services.CouponAPI.Models;
using Feasto.Services.CouponAPI.Models.DTO;

namespace Feasto.Services.CouponAPI;

public class MappingConfig
{
    public static MapperConfiguration RegisterMaps()
    {
        var mappingConfig = new MapperConfiguration(config =>
        {
            config.CreateMap<Coupon, CouponDTO>().ReverseMap();
        });
        return mappingConfig;
    }
}