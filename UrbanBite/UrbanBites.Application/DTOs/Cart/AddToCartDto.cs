namespace UrbanBites.Application.DTOs.Cart
{
    public class AddToCartDto
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }
}