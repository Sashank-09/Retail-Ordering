using UrbanBites.Domain.Entities;

namespace UrbanBites.Application.Interfaces.Repositories
{
    public interface IProductRepository : IGenericRepository<Product>
    {
        Task<IEnumerable<Product>> GetProductsByCategoryAsync(Guid categoryId);
        Task<IEnumerable<Product>> GetProductsByBrandAsync(Guid brandId);
        Task<IEnumerable<Product>> SearchAsync(string query);
    }
}