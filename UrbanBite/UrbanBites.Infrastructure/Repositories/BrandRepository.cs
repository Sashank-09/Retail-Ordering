using Microsoft.EntityFrameworkCore;
using UrbanBites.Application.Interfaces.Repositories;
using UrbanBites.Domain.Entities;
using UrbanBites.Infrastructure.Data;

namespace UrbanBites.Infrastructure.Repositories
{
    public class BrandRepository : GenericRepository<Brand>, IBrandRepository
    {
        public BrandRepository(AppDbContext context) : base(context) { }

        public async Task<Brand?> GetBrandWithCategoriesAsync(Guid brandId)
            => await _dbSet
                .Include(b => b.Categories)
                .FirstOrDefaultAsync(b => b.Id == brandId);
    }
}