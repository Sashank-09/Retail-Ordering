using UrbanBites.Application.DTOs.Loyalty;

namespace UrbanBites.Application.Interfaces.Services
{
    public interface ILoyaltyService
    {
        Task<LoyaltyBalanceDto> GetBalanceAsync(Guid userId);
        Task EarnPointsAsync(Guid userId, decimal orderAmount, string reason);
        Task<decimal> RedeemPointsAsync(Guid userId, int pointsToRedeem);
    }
}