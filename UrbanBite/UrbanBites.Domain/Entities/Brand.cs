namespace UrbanBites.Domain.Entities
{
    public class Brand : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string LogoUrl { get; set; } = string.Empty;

        // Navigation
        public ICollection<Category> Categories { get; set; } = new List<Category>();
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}