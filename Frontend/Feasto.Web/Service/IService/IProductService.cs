using Feasto.Web.Models;

namespace Feasto.Web.Service.IService;

public interface IProductService
{
    Task<ResponseDTO?> GetAllProductsAsync();
    Task<ResponseDTO?> GetProductByIdAsync(int id);
    Task<ResponseDTO?> CreateProductsAsync(ProductDTO productDto);
    Task<ResponseDTO?> UpdateProductsAsync(ProductDTO productDto);
    Task<ResponseDTO?> DeleteProductsAsync(int id);
}