using UrbanBites.Domain.Entities;

namespace UrbanBites.Application.Interfaces.Repositories
{
    public interface ICouponRepository : IGenericRepository<Coupon>
    {
        Task<Coupon?> GetByCodeAsync(string code);
    }
}