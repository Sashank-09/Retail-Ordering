namespace UrbanBites.Domain.Entities
{
    public class LoyaltyPoint : BaseEntity
    {
        public Guid UserId { get; set; }
        public int Points { get; set; }
        public string Reason { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty; // "Earned" or "Redeemed"
        public DateTime EarnedAt { get; set; } = DateTime.UtcNow;
    }
}