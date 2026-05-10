namespace UrbanBites.Application.DTOs.Cart
{
    public class CartDto
    {
        public Guid Id { get; set; }
        public List<CartItemDto> Items { get; set; } = new();
        public decimal TotalAmount => Items.Sum(i => i.SubTotal);
    }
}