using AutoMapper;
using Feasto.Services.ShoppingCartAPI.Models;
using Feasto.Services.ShoppingCartAPI.Models.DTO;

namespace Feasto.Services.ShoppingCartAPI;

public class MappingConfig
{
    public static MapperConfiguration RegisterMaps()
    {
        var mappingConfig = new MapperConfiguration(config =>
        {
            config.CreateMap<CartHeader, CartHeaderDTO>().ReverseMap();
            config.CreateMap<CartDetails, CartDetailsDTO>().ReverseMap();
        });
        return mappingConfig;
    }
}