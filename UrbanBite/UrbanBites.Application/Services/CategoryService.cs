using UrbanBites.Application.DTOs.Category;
using UrbanBites.Application.Interfaces.Repositories;
using UrbanBites.Application.Interfaces.Services;
using UrbanBites.Application.Mappings;

namespace UrbanBites.Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<IEnumerable<CategoryDto>> GetAllAsync()
        {
            var categories = await _categoryRepository.GetAllAsync();
            return categories.Select(c => c.ToDto());
        }

        public async Task<IEnumerable<CategoryDto>> GetByBrandAsync(Guid brandId)
        {
            var categories = await _categoryRepository.GetCategoriesByBrandAsync(brandId);
            return categories.Select(c => c.ToDto());
        }

        public async Task<CategoryDto?> GetByIdAsync(Guid id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            return category?.ToDto();
        }

        public async Task<CategoryDto> CreateAsync(CreateCategoryDto dto)
        {
            var category = dto.ToEntity();
            await _categoryRepository.AddAsync(category);
            await _categoryRepository.SaveChangesAsync();
            return category.ToDto();
        }

        public async Task<CategoryDto?> UpdateAsync(Guid id, UpdateCategoryDto dto)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category is null) return null;

            dto.UpdateEntity(category);
            _categoryRepository.Update(category);
            await _categoryRepository.SaveChangesAsync();
            return category.ToDto();
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category is null) return false;

            category.IsDeleted = true;
            _categoryRepository.Update(category);
            return await _categoryRepository.SaveChangesAsync();
        }
    }
}