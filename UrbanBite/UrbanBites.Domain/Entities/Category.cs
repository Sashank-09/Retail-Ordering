namespace UrbanBites.Domain.Entities
{
    public class Category : BaseEntity
    {
        public string Name { get; set; } = string.Empty;

        // Foreign Key
        public Guid BrandId { get; set; }

        // Navigation
        public Brand Brand { get; set; } = null!;
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}