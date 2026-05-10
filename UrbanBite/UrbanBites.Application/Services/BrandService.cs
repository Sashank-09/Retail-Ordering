using UrbanBites.Application.DTOs.Brand;
using UrbanBites.Application.Interfaces.Repositories;
using UrbanBites.Application.Interfaces.Services;
using UrbanBites.Application.Mappings;

namespace UrbanBites.Application.Services
{
    public class BrandService : IBrandService
    {
        private readonly IBrandRepository _brandRepository;

        public BrandService(IBrandRepository brandRepository)
        {
            _brandRepository = brandRepository;
        }

        public async Task<IEnumerable<BrandDto>> GetAllAsync()
        {
            var brands = await _brandRepository.GetAllAsync();
            return brands.Select(b => b.ToDto());
        }

        public async Task<BrandDto?> GetByIdAsync(Guid id)
        {
            var brand = await _brandRepository.GetByIdAsync(id);
            return brand?.ToDto();
        }

        public async Task<BrandDto> CreateAsync(CreateBrandDto dto)
        {
            var brand = dto.ToEntity();
            await _brandRepository.AddAsync(brand);
            await _brandRepository.SaveChangesAsync();
            return brand.ToDto();
        }

        public async Task<BrandDto?> UpdateAsync(Guid id, UpdateBrandDto dto)
        {
            var brand = await _brandRepository.GetByIdAsync(id);
            if (brand is null) return null;

            dto.UpdateEntity(brand);
            _brandRepository.Update(brand);
            await _brandRepository.SaveChangesAsync();
            return brand.ToDto();
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var brand = await _brandRepository.GetByIdAsync(id);
            if (brand is null) return false;

            brand.IsDeleted = true;
            _brandRepository.Update(brand);
            return await _brandRepository.SaveChangesAsync();
        }
    }
}