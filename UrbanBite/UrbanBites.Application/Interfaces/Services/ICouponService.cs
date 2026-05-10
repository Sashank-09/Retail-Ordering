using UrbanBites.Application.DTOs.Coupon;

namespace UrbanBites.Application.Interfaces.Services
{
    public interface ICouponService
    {
        Task<ApplyCouponResponseDto> ApplyAsync(ApplyCouponDto dto);
        Task<IEnumerable<CouponDto>> GetAllAsync();
        Task<CouponDto> CreateAsync(CouponDto dto);
        Task<bool> DeleteAsync(Guid id);
    }
}