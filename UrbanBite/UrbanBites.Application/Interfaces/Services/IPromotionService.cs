using UrbanBites.Application.DTOs.Promotion;

namespace UrbanBites.Application.Interfaces.Services
{
    public interface IPromotionService
    {
        Task<IEnumerable<PromotionDto>> GetActivePromotionsAsync();
        Task<IEnumerable<PromotionDto>> GetAllAsync();
        Task<PromotionDto> CreateAsync(PromotionDto dto);
        Task<bool> DeleteAsync(Guid id);
    }
}