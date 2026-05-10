using UrbanBites.Domain.Entities;

namespace UrbanBites.Application.Interfaces.Repositories
{
    public interface IBrandRepository : IGenericRepository<Brand>
    {
        Task<Brand?> GetBrandWithCategoriesAsync(Guid brandId);
    }
}