using Microsoft.EntityFrameworkCore;
using UrbanBites.Application.Interfaces.Repositories;
using UrbanBites.Domain.Entities;
using UrbanBites.Infrastructure.Data;

namespace UrbanBites.Infrastructure.Repositories
{
    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<Category>> GetCategoriesByBrandAsync(Guid brandId)
            => await _dbSet
                .Where(c => c.BrandId == brandId)
                .Include(c => c.Brand)
                .ToListAsync();
    }
}