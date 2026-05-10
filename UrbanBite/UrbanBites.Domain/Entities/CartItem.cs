namespace UrbanBites.Domain.Entities
{
    public class CartItem : BaseEntity
    {
        public int Quantity { get; set; }
        public DateTime AddedAt { get; set; } = DateTime.UtcNow;

        // Foreign Keys
        public Guid CartId { get; set; }
        public Guid ProductId { get; set; }

        // Navigation
        public Cart Cart { get; set; } = null!;
        public Product Product { get; set; } = null!;
    }
}