namespace UrbanBites.Application.DTOs.Payment
{
    public class PaymentCreateOrderDto
    {
        public decimal Amount { get; set; } // Amount in Rupees
    }

    public class PaymentVerifyDto
    {
        public string RazorpayPaymentId { get; set; } = string.Empty;
        public string RazorpayOrderId { get; set; } = string.Empty;
        public string RazorpaySignature { get; set; } = string.Empty;
    }
}
