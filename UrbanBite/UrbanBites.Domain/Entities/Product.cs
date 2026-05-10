namespace UrbanBites.Domain.Entities
{
    public class Product : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int StockCount { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public string Packaging { get; set; } = string.Empty;

        // Foreign Keys
        public Guid CategoryId { get; set; }
        public Guid BrandId { get; set; }

        // Navigation
        public Category Category { get; set; } = null!;
        public Brand Brand { get; set; } = null!;
        public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}