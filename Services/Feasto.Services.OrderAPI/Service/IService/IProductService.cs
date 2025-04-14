using Feasto.Services.OrderAPI.Models.DTO;

namespace Feasto.Services.OrderAPI.Service.IService;

public interface IProductService
{
    Task<IEnumerable<ProductDTO>> GetProducts();
}