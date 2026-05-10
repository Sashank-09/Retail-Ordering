namespace UrbanBites.Application.DTOs.Order
{
    public class PlaceOrderDto
    {
        public string DeliveryAddress { get; set; } = string.Empty;
        public string SpecialRequests { get; set; } = string.Empty;
        public string? CouponCode { get; set; }
        public bool UseLoyaltyPoints { get; set; } = false;
    }
}