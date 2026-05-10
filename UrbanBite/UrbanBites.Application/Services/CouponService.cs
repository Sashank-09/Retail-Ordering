using UrbanBites.Application.DTOs.Coupon;
using UrbanBites.Application.Interfaces.Repositories;
using UrbanBites.Application.Interfaces.Services;
using UrbanBites.Domain.Entities;

namespace UrbanBites.Application.Services
{
    public class CouponService : ICouponService
    {
        private readonly ICouponRepository _couponRepository;

        public CouponService(ICouponRepository couponRepository)
        {
            _couponRepository = couponRepository;
        }

        public async Task<ApplyCouponResponseDto> ApplyAsync(ApplyCouponDto dto)
        {
            var coupon = await _couponRepository.GetByCodeAsync(dto.Code);

            if (coupon is null)
                throw new Exception("Invalid or expired coupon code.");
            if (coupon.UsedCount >= coupon.UsageLimit)
                throw new Exception("Coupon usage limit reached.");
            if (dto.OrderAmount < coupon.MinOrderAmount)
                throw new Exception(
                    $"Minimum order amount ₹{coupon.MinOrderAmount} required.");

            var discountAmount = dto.OrderAmount * coupon.DiscountPercent / 100;

            return new ApplyCouponResponseDto
            {
                Code = coupon.Code,
                DiscountPercent = coupon.DiscountPercent,
                DiscountAmount = discountAmount,
                FinalAmount = dto.OrderAmount - discountAmount
            };
        }

        public async Task<IEnumerable<CouponDto>> GetAllAsync()
        {
            var coupons = await _couponRepository.GetAllAsync();
            return coupons.Select(ToDto);
        }

        public async Task<CouponDto> CreateAsync(CouponDto dto)
        {
            var coupon = new Coupon
            {
                Id = Guid.NewGuid(),
                Code = dto.Code.ToUpper(),
                Description = dto.Description,
                DiscountPercent = dto.DiscountPercent,
                ExpiryDate = dto.ExpiryDate,
                MinOrderAmount = dto.MinOrderAmount,
                UsageLimit = 100,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _couponRepository.AddAsync(coupon);
            await _couponRepository.SaveChangesAsync();
            dto.Id = coupon.Id;
            return dto;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var coupon = await _couponRepository.GetByIdAsync(id);
            if (coupon is null) return false;

            coupon.IsDeleted = true;
            _couponRepository.Update(coupon);
            return await _couponRepository.SaveChangesAsync();
        }

        private static CouponDto ToDto(Coupon c) => new()
        {
            Id = c.Id,
            Code = c.Code,
            Description = c.Description,
            DiscountPercent = c.DiscountPercent,
            ExpiryDate = c.ExpiryDate,
            MinOrderAmount = c.MinOrderAmount
        };
    }
}