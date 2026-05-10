using UrbanBites.Domain.Enums;

namespace UrbanBites.Domain.Entities
{
    public class Order : BaseEntity
    {
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public decimal TotalAmount { get; set; }
        public string DeliveryAddress { get; set; } = string.Empty;
        public string SpecialRequests { get; set; } = string.Empty;
        public string? CouponCode { get; set; }
        public decimal DiscountAmount { get; set; } = 0;
        public int LoyaltyPointsEarned { get; set; } = 0;
        public int LoyaltyPointsUsed { get; set; } = 0; public DateTime PlacedAt { get; set; } = DateTime.UtcNow;

        // Foreign Key only — no navigation to AppUser in Domain
        public Guid UserId { get; set; }

        // Navigation
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}