using Microsoft.EntityFrameworkCore;
using UrbanBites.Application.Interfaces.Repositories;
using UrbanBites.Domain.Entities;
using UrbanBites.Infrastructure.Data;

namespace UrbanBites.Infrastructure.Repositories
{
    public class LoyaltyRepository : ILoyaltyRepository
    {
        private readonly AppDbContext _context;

        public LoyaltyRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<LoyaltyPoint>> GetByUserIdAsync(Guid userId)
            => await _context.LoyaltyPoints
                .Where(l => l.UserId == userId)
                .OrderByDescending(l => l.EarnedAt)
                .ToListAsync();

        public async Task AddAsync(LoyaltyPoint loyaltyPoint)
            => await _context.LoyaltyPoints.AddAsync(loyaltyPoint);

        public async Task<bool> SaveChangesAsync()
            => await _context.SaveChangesAsync() > 0;
    }
}