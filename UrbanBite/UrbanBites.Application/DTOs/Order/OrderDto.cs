using UrbanBites.Domain.Enums;

namespace UrbanBites.Application.DTOs.Order
{
    public class OrderDto
    {
        public Guid Id { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public string? CouponCode { get; set; }
        public decimal DiscountAmount { get; set; }
        public int LoyaltyPointsEarned { get; set; }
        public int LoyaltyPointsUsed { get; set; }
        public string DeliveryAddress { get; set; } = string.Empty;
        public string SpecialRequests { get; set; } = string.Empty;
        public DateTime PlacedAt { get; set; }
        public List<OrderItemDto> Items { get; set; } = new();
    }
}