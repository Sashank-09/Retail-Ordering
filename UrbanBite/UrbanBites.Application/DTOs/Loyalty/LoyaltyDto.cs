namespace UrbanBites.Application.DTOs.Loyalty
{
    public class LoyaltyBalanceDto
    {
        public int TotalPoints { get; set; }
        public decimal EquivalentAmount { get; set; } // 10 points = ₹1
        public List<LoyaltyTransactionDto> Transactions { get; set; } = new();
    }

    public class LoyaltyTransactionDto
    {
        public int Points { get; set; }
        public string Reason { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public DateTime EarnedAt { get; set; }
    }

    public class RedeemLoyaltyDto
    {
        public int PointsToRedeem { get; set; }
    }
}