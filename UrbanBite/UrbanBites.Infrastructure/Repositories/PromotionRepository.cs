using Microsoft.EntityFrameworkCore;
using UrbanBites.Application.Interfaces.Repositories;
using UrbanBites.Domain.Entities;
using UrbanBites.Infrastructure.Data;

namespace UrbanBites.Infrastructure.Repositories
{
    public class PromotionRepository : GenericRepository<Promotion>,
                                       IPromotionRepository
    {
        public PromotionRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<Promotion>> GetActivePromotionsAsync()
        {
            var now = DateTime.UtcNow;
            return await _dbSet
                .Where(p => p.IsActive &&
                            p.StartDate <= now &&
                            p.EndDate >= now)
                .ToListAsync();
        }

        public async Task<IEnumerable<Promotion>> GetAllWithDeletedAsync()
            => await _dbSet
                .IgnoreQueryFilters()
                .ToListAsync();
    }
}