namespace UrbanBites.Application.Interfaces.Services
{
    public interface IEmailService
    {
        Task SendOrderConfirmationAsync(
    string toEmail,
    string customerName,
    string orderId,
    decimal totalAmount,
    decimal discountAmount,
    string deliveryAddress,
    List<OrderEmailItem> items,
    string? transactionId = null);

        Task SendOtpEmailAsync(string toEmail, string otpCode);
        Task SendWelcomeEmailAsync(string toEmail, string customerName);
    }

    public class OrderEmailItem
    {
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal SubTotal { get; set; }
    }
}