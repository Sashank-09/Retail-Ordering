using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;
using UrbanBites.Application.Interfaces.Services;

namespace UrbanBites.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendOrderConfirmationAsync(
            string toEmail,
            string customerName,
            string orderId,
            decimal totalAmount,
            decimal discountAmount,
            string deliveryAddress,
            List<OrderEmailItem> items,
            string? transactionId = null)
        {
            try
            {
                var apiKey = _configuration["EmailSettings:SendGridApiKey"]!;
                var senderEmail = _configuration["EmailSettings:SenderEmail"]!;
                var senderName = _configuration["EmailSettings:SenderName"]!;

                var itemsHtml = string.Join("", items.Select(item => $"""
                    <tr>
                      <td style="padding:10px 0;border-bottom:1px solid #f0f0f0;color:#333;font-size:14px;">{item.ProductName}</td>
                      <td style="padding:10px 0;border-bottom:1px solid #f0f0f0;color:#666;font-size:14px;text-align:center;">× {item.Quantity}</td>
                      <td style="padding:10px 0;border-bottom:1px solid #f0f0f0;color:#333;font-size:14px;text-align:right;font-weight:600;">₹{item.SubTotal}</td>
                    </tr>
                """));

                var discountRow = discountAmount > 0 ? $"""
                    <tr>
                      <td colspan="2" style="padding:8px 0;color:#4caf50;font-size:14px;">Discount Applied</td>
                      <td style="padding:8px 0;color:#4caf50;font-size:14px;text-align:right;font-weight:600;">− ₹{discountAmount}</td>
                    </tr>
                """ : "";

                var paymentRow = !string.IsNullOrEmpty(transactionId) ? $"""
                    <tr>
                      <td colspan="2" style="padding:8px 0;color:#555;font-size:13px;">💳 Payment ID</td>
                      <td style="padding:8px 0;color:#555;font-size:13px;text-align:right;font-family:monospace;">{transactionId}</td>
                    </tr>
                """ : "";

                var html = $"""
                    <html>
                    <body style="font-family:Arial,sans-serif;background:#f4f4f4;padding:20px;">
                      <div style="background:white;padding:30px;border-radius:12px;max-width:600px;margin:auto;">
                        <h1 style="color:#FF6B35;">🍕 UrbanBites</h1>
                        <h2 style="color:#2e7d32;">✅ Order Confirmed!</h2>
                        <p>Hi <b>{customerName}</b>, your order <b>#{orderId[..8].ToUpper()}</b> has been placed successfully.</p>
                        <hr style="border:none;border-top:1px solid #eee;margin:20px 0;">
                        <table width="100%">{itemsHtml}{discountRow}{paymentRow}</table>
                        <p style="font-size:18px;font-weight:bold;margin-top:20px;">Total Paid: ₹{totalAmount}</p>
                        <p style="color:#666;">📍 Delivering to: {deliveryAddress}</p>
                        <div style="margin-top:30px;text-align:center;">
                           <a href="https://retail-ordering.vercel.app/my-bookings" style="background:#FF6B35;color:white;padding:12px 24px;text-decoration:none;border-radius:8px;font-weight:bold;">Track Order</a>
                        </div>
                      </div>
                    </body>
                    </html>
                """;

                var client = new SendGridClient(apiKey);
                var from = new EmailAddress(senderEmail, senderName);
                var to = new EmailAddress(toEmail, customerName);
                var subject = $"UrbanBites — Order #{orderId[..8].ToUpper()} Confirmed! 🎉";
                var msg = MailHelper.CreateSingleEmail(from, to, subject, "Order Confirmed", html);

                var response = await client.SendEmailAsync(msg);
                if (!response.IsSuccessStatusCode)
                {
                    var body = await response.Body.ReadAsStringAsync();
                    throw new Exception($"SendGrid Error: {response.StatusCode} - {body}");
                }
                Console.WriteLine($"[EMAIL] ✅ Order confirmation sent to {toEmail}");
            }
            catch (Exception ex)
            {
                await SaveToLocalFile("OrderConfirmation", toEmail, ex.Message);
            }
        }

        public async Task SendOtpEmailAsync(string toEmail, string otpCode)
        {
            try
            {
                var apiKey = _configuration["EmailSettings:SendGridApiKey"]!;
                var senderEmail = _configuration["EmailSettings:SenderEmail"]!;
                var senderName = _configuration["EmailSettings:SenderName"]!;

                var html = $"""
                    <html>
                    <body style="font-family:Arial,sans-serif;text-align:center;padding:40px;">
                      <h1 style="color:#FF6B35;">UrbanBites</h1>
                      <h2>Your OTP Code</h2>
                      <div style="font-size:32px;font-weight:bold;letter-spacing:10px;color:#FF6B35;background:#fef3c7;padding:20px;border-radius:12px;display:inline-block;margin:20px 0;">
                        {otpCode}
                      </div>
                      <p style="color:#666;">This code expires in 10 minutes. Do not share it with anyone.</p>
                    </body>
                    </html>
                """;

                var client = new SendGridClient(apiKey);
                var from = new EmailAddress(senderEmail, senderName);
                var to = new EmailAddress(toEmail);
                var subject = "UrbanBites — Password Reset OTP";
                var msg = MailHelper.CreateSingleEmail(from, to, subject, $"Your OTP: {otpCode}", html);

                var response = await client.SendEmailAsync(msg);
                if (!response.IsSuccessStatusCode)
                {
                    var body = await response.Body.ReadAsStringAsync();
                    throw new Exception($"SendGrid Error: {response.StatusCode} - {body}");
                }
                Console.WriteLine($"[EMAIL] ✅ OTP email sent to {toEmail}");
            }
            catch (Exception ex)
            {
                await SaveToLocalFile("OTP", toEmail, ex.Message);
            }
        }

        public async Task SendWelcomeEmailAsync(string toEmail, string customerName)
        {
            try
            {
                var apiKey = _configuration["EmailSettings:SendGridApiKey"]!;
                var senderEmail = _configuration["EmailSettings:SenderEmail"]!;
                var senderName = _configuration["EmailSettings:SenderName"]!;

                var html = $"""
                    <html>
                    <body style="font-family:Arial,sans-serif;padding:40px;">
                      <div style="max-width:600px;margin:auto;border:1px solid #eee;border-radius:12px;padding:30px;">
                        <h1 style="color:#FF6B35;">🍕 Welcome to UrbanBites!</h1>
                        <p>Hi <b>{customerName}</b>,</p>
                        <p>We're thrilled to have you! Start exploring fresh food from top brands and get it delivered fast.</p>
                        <p style="margin:30px 0;text-align:center;">
                          <a href="https://retail-ordering.vercel.app/products" style="background:#FF6B35;color:white;padding:14px 28px;text-decoration:none;border-radius:8px;font-weight:bold;">Browse Menu</a>
                        </p>
                        <p style="color:#888;font-size:12px;border-top:1px solid #eee;padding-top:20px;">Use code <b>WELCOME20</b> for 20% off your first order!</p>
                      </div>
                    </body>
                    </html>
                """;

                var client = new SendGridClient(apiKey);
                var from = new EmailAddress(senderEmail, senderName);
                var to = new EmailAddress(toEmail, customerName);
                var subject = "Welcome to UrbanBites! 🍕";
                var msg = MailHelper.CreateSingleEmail(from, to, subject, "Welcome to UrbanBites!", html);

                var response = await client.SendEmailAsync(msg);
                if (!response.IsSuccessStatusCode)
                {
                    var body = await response.Body.ReadAsStringAsync();
                    throw new Exception($"SendGrid Error: {response.StatusCode} - {body}");
                }
                Console.WriteLine($"[EMAIL] ✅ Welcome email sent to {toEmail}");
            }
            catch (Exception ex)
            {
                await SaveToLocalFile("Welcome", toEmail, ex.Message);
            }
        }

        public async Task SendOrderStatusUpdateEmailAsync(string toEmail, string customerName, string orderId, string newStatus)
        {
            try
            {
                var apiKey = _configuration["EmailSettings:SendGridApiKey"]!;
                var senderEmail = _configuration["EmailSettings:SenderEmail"]!;
                var senderName = _configuration["EmailSettings:SenderName"]!;

                // Pretty names for status updates
                string statusDesc = newStatus switch
                {
                    "Confirmed" => "is now confirmed and being reviewed by the kitchen.",
                    "Preparing" => "is being freshly prepared by our chefs!",
                    "OutForDelivery" => "is out for delivery and will arrive shortly 🛵",
                    "Delivered" => "has been successfully delivered. Enjoy your meal!",
                    "Cancelled" => "has been cancelled.",
                    _ => $"status has changed to {newStatus}."
                };

                var html = $"""
                    <html>
                    <body style="font-family:Arial,sans-serif;padding:40px;background:#f9fafb;">
                      <div style="max-width:600px;margin:auto;background:white;border-radius:16px;padding:30px;box-shadow:0 4px 6px -1px rgba(0,0,0,0.1);">
                        <div style="text-align:center;margin-bottom:20px;">
                           <h1 style="color:#FF6B35;margin:0;">🍕 UrbanBites</h1>
                        </div>
                        <h2>Order Update</h2>
                        <p>Hi <b>{customerName}</b>,</p>
                        <p style="font-size:16px;line-height:1.5;">Your order <b>#{orderId[..8].ToUpper()}</b> {statusDesc}</p>
                        <div style="margin-top:30px;text-align:center;border-top:1px solid #eee;padding-top:20px;">
                           <a href="https://retail-ordering.vercel.app/my-bookings" style="background:#FF6B35;color:white;padding:12px 24px;text-decoration:none;border-radius:8px;font-weight:bold;display:inline-block;">View Order Progress</a>
                        </div>
                      </div>
                    </body>
                    </html>
                """;

                var client = new SendGridClient(apiKey);
                var from = new EmailAddress(senderEmail, senderName);
                var to = new EmailAddress(toEmail, customerName);
                var subject = $"UrbanBites — Order Status Update: {newStatus}";
                var msg = MailHelper.CreateSingleEmail(from, to, subject, $"Order #{orderId[..8].ToUpper()} {newStatus}", html);

                var response = await client.SendEmailAsync(msg);
                if (!response.IsSuccessStatusCode)
                {
                    var body = await response.Body.ReadAsStringAsync();
                    throw new Exception($"SendGrid Error: {response.StatusCode} - {body}");
                }
                Console.WriteLine($"[EMAIL] ✅ Order status email sent to {toEmail}");
            }
            catch (Exception ex)
            {
                await SaveToLocalFile("StatusUpdate", toEmail, ex.Message);
            }
        }

        private async Task SaveToLocalFile(string type, string to, string error)
        {
            try
            {
                var dir = Path.Combine(Directory.GetCurrentDirectory(), "temp_emails");
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

                var fileName = $"{type}_{DateTime.Now:yyyyMMdd_HHmmss}_{Guid.NewGuid().ToString()[..4]}.html";
                var path = Path.Combine(dir, fileName);

                var logMsg = $"[EMAIL] ❌ {type} to {to} FAILED via SendGrid. Error: {error}. Saved locally to: {path}";
                Console.WriteLine(logMsg);

                var content = $"""
                    <html>
                    <body style="font-family:sans-serif;padding:20px;">
                      <div style="background:#fff1f2;border:1px solid #fda4af;padding:20px;border-radius:8px;">
                        <h2 style="color:#be123c;margin-top:0;">Email Delivery Failed (SendGrid)</h2>
                        <p><b>Target:</b> {to}</p>
                        <p><b>Type:</b> {type}</p>
                        <p><b>Error Details:</b> {error}</p>
                        <hr>
                        <p style="font-size:12px;color:#666;">Note: In production, ensure the Sender Email is verified in SendGrid and the API Key is valid.</p>
                      </div>
                    </body>
                    </html>
                """;
                await File.WriteAllTextAsync(path, content);
            }
            catch (Exception logEx)
            {
                Console.WriteLine($"[EMAIL] CRITICAL: Failed to even save fallback file: {logEx.Message}");
            }
        }
    }
}