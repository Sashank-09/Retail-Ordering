namespace UrbanBites.Application.DTOs.Coupon
{
    public class CouponDto
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal DiscountPercent { get; set; }
        public DateTime ExpiryDate { get; set; }
        public decimal MinOrderAmount { get; set; }
    }

    public class ApplyCouponDto
    {
        public string Code { get; set; } = string.Empty;
        public decimal OrderAmount { get; set; }
    }

    public class ApplyCouponResponseDto
    {
        public string Code { get; set; } = string.Empty;
        public decimal DiscountPercent { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal FinalAmount { get; set; }
    }
}