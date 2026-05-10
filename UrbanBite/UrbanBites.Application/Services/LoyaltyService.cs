using UrbanBites.Application.DTOs.Loyalty;
using UrbanBites.Application.Interfaces.Repositories;
using UrbanBites.Application.Interfaces.Services;
using UrbanBites.Domain.Entities;

namespace UrbanBites.Application.Services
{
    public class LoyaltyService : ILoyaltyService
    {
        private readonly ILoyaltyRepository _loyaltyRepository;
        private const int RupeesPerPoint = 10; // 10 points = ₹1

        public LoyaltyService(ILoyaltyRepository loyaltyRepository)
        {
            _loyaltyRepository = loyaltyRepository;
        }

        public async Task<LoyaltyBalanceDto> GetBalanceAsync(Guid userId)
        {
            var records = await _loyaltyRepository.GetByUserIdAsync(userId);

            var totalPoints = records
                .Sum(l => l.Type == "Earned" ? l.Points : -l.Points);

            return new LoyaltyBalanceDto
            {
                TotalPoints = Math.Max(0, totalPoints),
                EquivalentAmount = Math.Max(0, totalPoints) / RupeesPerPoint,
                Transactions = records.Select(l => new LoyaltyTransactionDto
                {
                    Points = l.Points,
                    Reason = l.Reason,
                    Type = l.Type,
                    EarnedAt = l.EarnedAt
                }).ToList()
            };
        }

        public async Task EarnPointsAsync(Guid userId,
                                           decimal orderAmount,
                                           string reason)
        {
            var points = (int)orderAmount; // ₹1 = 1 point
            if (points <= 0) return;

            await _loyaltyRepository.AddAsync(new LoyaltyPoint
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Points = points,
                Reason = reason,
                Type = "Earned",
                EarnedAt = DateTime.UtcNow
            });

            await _loyaltyRepository.SaveChangesAsync();
        }

        public async Task<decimal> RedeemPointsAsync(Guid userId, int pointsToRedeem)
        {
            var balance = await GetBalanceAsync(userId);

            if (pointsToRedeem > balance.TotalPoints)
                throw new Exception(
                    $"Insufficient points. Available: {balance.TotalPoints}");

            await _loyaltyRepository.AddAsync(new LoyaltyPoint
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Points = pointsToRedeem,
                Reason = "Redeemed on order",
                Type = "Redeemed",
                EarnedAt = DateTime.UtcNow
            });

            await _loyaltyRepository.SaveChangesAsync();
            return (decimal)pointsToRedeem / RupeesPerPoint;
        }
    }
}