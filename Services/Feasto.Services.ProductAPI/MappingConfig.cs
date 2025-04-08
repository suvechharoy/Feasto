using AutoMapper;
using Feasto.Services.ProductAPI.Models;
using Feasto.Services.ProductAPI.Models.DTO;

namespace Feasto.Services.ProductAPI;

public class MappingConfig
{
    public static MapperConfiguration RegisterMaps()
    {
        var mappingConfig = new MapperConfiguration(config =>
        {
            config.CreateMap<Product, ProductDTO>().ReverseMap();
        });
        return mappingConfig;
    }
}