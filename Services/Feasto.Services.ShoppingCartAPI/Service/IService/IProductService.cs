using Feasto.Services.ShoppingCartAPI.Models.DTO;

namespace Feasto.Services.ShoppingCartAPI.Service.IService;

public interface IProductService
{
    Task<IEnumerable<ProductDTO>> GetProducts();
}