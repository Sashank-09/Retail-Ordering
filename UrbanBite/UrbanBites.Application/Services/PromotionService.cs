using UrbanBites.Application.DTOs.Promotion;
using UrbanBites.Application.Interfaces.Repositories;
using UrbanBites.Application.Interfaces.Services;
using UrbanBites.Domain.Entities;

namespace UrbanBites.Application.Services
{
    public class PromotionService : IPromotionService
    {
        private readonly IPromotionRepository _promotionRepository;

        public PromotionService(IPromotionRepository promotionRepository)
        {
            _promotionRepository = promotionRepository;
        }

        public async Task<IEnumerable<PromotionDto>> GetActivePromotionsAsync()
        {
            var promos = await _promotionRepository.GetActivePromotionsAsync();
            return promos.Select(ToDto);
        }

        public async Task<IEnumerable<PromotionDto>> GetAllAsync()
        {
            var promos = await _promotionRepository.GetAllWithDeletedAsync();
            return promos.Select(ToDto);
        }

        public async Task<PromotionDto> CreateAsync(PromotionDto dto)
        {
            var promotion = new Promotion
            {
                Id = Guid.NewGuid(),
                Title = dto.Title,
                Description = dto.Description,
                DiscountPercent = dto.DiscountPercent,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                ImageUrl = dto.ImageUrl,
                IsActive = dto.IsActive,
                CreatedAt = DateTime.UtcNow
            };

            await _promotionRepository.AddAsync(promotion);
            await _promotionRepository.SaveChangesAsync();
            dto.Id = promotion.Id;
            return dto;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var promo = await _promotionRepository.GetByIdAsync(id);
            if (promo is null) return false;

            promo.IsDeleted = true;
            _promotionRepository.Update(promo);
            return await _promotionRepository.SaveChangesAsync();
        }

        private static PromotionDto ToDto(Promotion p) => new()
        {
            Id = p.Id,
            Title = p.Title,
            Description = p.Description,
            DiscountPercent = p.DiscountPercent,
            StartDate = p.StartDate,
            EndDate = p.EndDate,
            ImageUrl = p.ImageUrl,
            IsActive = p.IsActive
        };
    }
}