using UrbanBites.Application.DTOs.Category;

namespace UrbanBites.Application.Interfaces.Services
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryDto>> GetAllAsync();
        Task<IEnumerable<CategoryDto>> GetByBrandAsync(Guid brandId);
        Task<CategoryDto?> GetByIdAsync(Guid id);
        Task<CategoryDto> CreateAsync(CreateCategoryDto dto);
        Task<CategoryDto?> UpdateAsync(Guid id, UpdateCategoryDto dto);
        Task<bool> DeleteAsync(Guid id);
    }
}