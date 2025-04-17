using Feasto.Web.Models;
using Feasto.Web.Service.IService;
using Feasto.Web.Utility;

namespace Feasto.Web.Service;

public class ProductService : IProductService
{
    private readonly IBaseService _baseService;

    public ProductService(IBaseService baseService)
    {
        _baseService = baseService;
    }

    public async Task<ResponseDTO?> GetAllProductsAsync()
    {
        return await _baseService.SendAsync(new RequestDTO()
        {
            ApiType = StaticDetails.ApiType.GET,
            Url = StaticDetails.ProductAPIBase + "/api/product"
        });
    }

    public async Task<ResponseDTO?> GetProductByIdAsync(int id)
    {
        return await _baseService.SendAsync(new RequestDTO()
        {
            ApiType = StaticDetails.ApiType.GET,
            Url = StaticDetails.ProductAPIBase + "/api/product/"+id
        });
    }

    public async Task<ResponseDTO?> CreateProductsAsync(ProductDTO productDto)
    {
        return await _baseService.SendAsync(new RequestDTO()
        {
            ApiType = StaticDetails.ApiType.POST,
            Data = productDto,
            Url = StaticDetails.ProductAPIBase + "/api/product",
            ContentType = StaticDetails.ContentType.MultipartFormData
        });
    }

    public async Task<ResponseDTO?> UpdateProductsAsync(ProductDTO productDto)
    {
        return await _baseService.SendAsync(new RequestDTO()
        {
            ApiType = StaticDetails.ApiType.PUT,
            Data = productDto,
            Url = StaticDetails.ProductAPIBase + "/api/product",
            ContentType = StaticDetails.ContentType.MultipartFormData
        });
    }

    public async Task<ResponseDTO?> DeleteProductsAsync(int id)
    {
        return await _baseService.SendAsync(new RequestDTO()
        {
            ApiType = StaticDetails.ApiType.DELETE,
            Url = StaticDetails.ProductAPIBase + "/api/product/"+id
        });
    }
}