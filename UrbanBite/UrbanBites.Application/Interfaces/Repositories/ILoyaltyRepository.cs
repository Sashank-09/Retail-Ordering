using UrbanBites.Domain.Entities;

namespace UrbanBites.Application.Interfaces.Repositories
{
    public interface ILoyaltyRepository
    {
        Task<IEnumerable<LoyaltyPoint>> GetByUserIdAsync(Guid userId);
        Task AddAsync(LoyaltyPoint loyaltyPoint);
        Task<bool> SaveChangesAsync();
    }
}