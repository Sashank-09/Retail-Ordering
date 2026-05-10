using UrbanBites.Application.DTOs.Product;
using UrbanBites.Application.Interfaces.Repositories;
using UrbanBites.Application.Interfaces.Services;
using UrbanBites.Application.Mappings;

namespace UrbanBites.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<IEnumerable<ProductDto>> GetAllAsync()
        {
            var products = await _productRepository.GetAllAsync();
            return products.Select(p => p.ToDto());
        }

        public async Task<IEnumerable<ProductDto>> GetByCategoryAsync(Guid categoryId)
        {
            var products = await _productRepository.GetProductsByCategoryAsync(categoryId);
            return products.Select(p => p.ToDto());
        }

        public async Task<IEnumerable<ProductDto>> GetByBrandAsync(Guid brandId)
        {
            var products = await _productRepository.GetProductsByBrandAsync(brandId);
            return products.Select(p => p.ToDto());
        }

        public async Task<ProductDto?> GetByIdAsync(Guid id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            return product?.ToDto();
        }

        public async Task<ProductDto> CreateAsync(CreateProductDto dto)
        {
            var product = dto.ToEntity();
            await _productRepository.AddAsync(product);
            await _productRepository.SaveChangesAsync();
            return product.ToDto();
        }

        public async Task<ProductDto?> UpdateAsync(Guid id, UpdateProductDto dto)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product is null) return null;

            dto.UpdateEntity(product);
            _productRepository.Update(product);
            await _productRepository.SaveChangesAsync();
            return product.ToDto();
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product is null) return false;

            product.IsDeleted = true;
            _productRepository.Update(product);
            return await _productRepository.SaveChangesAsync();
        }

        public async Task<IEnumerable<ProductDto>> SearchAsync(string query)
        {
            var products = await _productRepository.SearchAsync(query);
            return products.Select(p => p.ToDto());
        }
    }
}