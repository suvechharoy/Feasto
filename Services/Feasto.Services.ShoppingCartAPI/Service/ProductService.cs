using Feasto.Services.ShoppingCartAPI.Models.DTO;
using Feasto.Services.ShoppingCartAPI.Service.IService;
using Newtonsoft.Json;

namespace Feasto.Services.ShoppingCartAPI.Service;

public class ProductService : IProductService
{
    private readonly IHttpClientFactory _httpClientFactory;

    public ProductService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }
    
    public async Task<IEnumerable<ProductDTO>> GetProducts()
    {
        var client = _httpClientFactory.CreateClient("Product");
        var result = await client.GetAsync("/api/product");
        var apiContent = await result.Content.ReadAsStringAsync();
        var response = JsonConvert.DeserializeObject<ResponseDTO>(apiContent);
        if (response.IsSuccess)
        {
            return JsonConvert.DeserializeObject<IEnumerable<ProductDTO>>(Convert.ToString(response.Result));
        }
        return new List<ProductDTO>();
    }
}