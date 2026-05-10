using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Razorpay.Api;
using UrbanBites.Application.DTOs.Payment;

namespace UrbanBites.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PaymentController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public PaymentController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("create-order")]
        public IActionResult CreateOrder([FromBody] PaymentCreateOrderDto dto)
        {
            try
            {
                var keyId = _configuration["Razorpay:KeyId"];
                var keySecret = _configuration["Razorpay:KeySecret"];

                if (string.IsNullOrEmpty(keyId) || string.IsNullOrEmpty(keySecret))
                {
                    return StatusCode(500, new { message = "Razorpay keys not configured." });
                }

                RazorpayClient client = new RazorpayClient(keyId, keySecret);

                Dictionary<string, object> options = new Dictionary<string, object>();
                // Amount must be in paise (multiply by 100)
                options.Add("amount", (int)(dto.Amount * 100)); 
                options.Add("currency", "INR");
                options.Add("receipt", Guid.NewGuid().ToString().Substring(0, 20));

                Order order = client.Order.Create(options);
                string orderId = order["id"].ToString();

                return Ok(new { razorpayOrderId = orderId });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("verify")]
        public IActionResult VerifyPayment([FromBody] PaymentVerifyDto dto)
        {
            try
            {
                var keySecret = _configuration["Razorpay:KeySecret"];

                Dictionary<string, string> attributes = new Dictionary<string, string>
                {
                    { "razorpay_payment_id", dto.RazorpayPaymentId },
                    { "razorpay_order_id", dto.RazorpayOrderId },
                    { "razorpay_signature", dto.RazorpaySignature }
                };

                Utils.verifyPaymentSignature(attributes);
                
                return Ok(new { success = true, transactionId = dto.RazorpayPaymentId });
            }
            catch (Exception)
            {
                return BadRequest(new { success = false, message = "Payment verification failed. Invalid signature." });
            }
        }
    }
}
