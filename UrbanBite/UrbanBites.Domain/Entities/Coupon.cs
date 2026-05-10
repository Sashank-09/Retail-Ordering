namespace UrbanBites.Domain.Entities
{
    public class Coupon : BaseEntity
    {
        public string Code { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal DiscountPercent { get; set; }
        public DateTime ExpiryDate { get; set; }
        public int UsageLimit { get; set; }
        public int UsedCount { get; set; } = 0;
        public bool IsActive { get; set; } = true;
        public decimal MinOrderAmount { get; set; } = 0;
    }
}