using UrbanBites.Application.DTOs.Product;

namespace UrbanBites.Application.Interfaces.Services
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetAllAsync();
        Task<IEnumerable<ProductDto>> GetByCategoryAsync(Guid categoryId);
        Task<IEnumerable<ProductDto>> GetByBrandAsync(Guid brandId);
        Task<ProductDto?> GetByIdAsync(Guid id);
        Task<ProductDto> CreateAsync(CreateProductDto dto);
        Task<ProductDto?> UpdateAsync(Guid id, UpdateProductDto dto);
        Task<bool> DeleteAsync(Guid id);
        Task<IEnumerable<ProductDto>> SearchAsync(string query);
    }
}