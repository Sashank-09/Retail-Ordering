using UrbanBites.Domain.Entities;

namespace UrbanBites.Application.Interfaces.Repositories
{
    public interface ICategoryRepository : IGenericRepository<Category>
    {
        Task<IEnumerable<Category>> GetCategoriesByBrandAsync(Guid brandId);
    }
}