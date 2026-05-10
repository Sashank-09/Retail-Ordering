using Microsoft.EntityFrameworkCore;
using UrbanBites.Application.Interfaces.Repositories;
using UrbanBites.Domain.Entities;
using UrbanBites.Infrastructure.Data;

namespace UrbanBites.Infrastructure.Repositories
{
    public class CouponRepository : GenericRepository<Coupon>, ICouponRepository
    {
        public CouponRepository(AppDbContext context) : base(context) { }

        public async Task<Coupon?> GetByCodeAsync(string code)
            => await _dbSet
                .FirstOrDefaultAsync(c =>
                    c.Code.ToUpper() == code.ToUpper() &&
                    c.IsActive &&
                    c.ExpiryDate >= DateTime.UtcNow);
    }
}