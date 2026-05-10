namespace UrbanBites.Domain.Entities
{
    public class Cart : BaseEntity
    {
        // Foreign Key only — no navigation to AppUser in Domain
        public Guid UserId { get; set; }

        // Navigation
        public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    }
}