using UrbanBites.Application.DTOs.Brand;

namespace UrbanBites.Application.Interfaces.Services
{
    public interface IBrandService
    {
        Task<IEnumerable<BrandDto>> GetAllAsync();
        Task<BrandDto?> GetByIdAsync(Guid id);
        Task<BrandDto> CreateAsync(CreateBrandDto dto);
        Task<BrandDto?> UpdateAsync(Guid id, UpdateBrandDto dto);
        Task<bool> DeleteAsync(Guid id);
    }
}