using UrbanBites.Domain.Entities;

namespace UrbanBites.Application.Interfaces.Repositories
{
    public interface IPromotionRepository : IGenericRepository<Promotion>
    {
        Task<IEnumerable<Promotion>> GetActivePromotionsAsync();
        Task<IEnumerable<Promotion>> GetAllWithDeletedAsync();
    }
}