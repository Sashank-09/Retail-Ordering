using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;
using UrbanBites.Application.DTOs.Order;
using UrbanBites.Application.Interfaces.Services;

namespace UrbanBites.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    [EnableRateLimiting("GeneralPolicy")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        private Guid GetUserId() =>
            Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        private string GetUserEmail() =>
            User.FindFirstValue(ClaimTypes.Email) ?? string.Empty;

        private string GetUserName() =>
            User.FindFirstValue(ClaimTypes.Name) ?? string.Empty;

        [HttpPost("place")]
        [EnableRateLimiting("OrderPolicy")]
        public async Task<IActionResult> PlaceOrder([FromBody] PlaceOrderDto dto)
        {
            try
            {
                var userId = GetUserId();
                var userEmail = GetUserEmail();
                var userName = GetUserName();

                // Debug — check these are populated
                Console.WriteLine($"[ORDER] UserId: {userId}");
                Console.WriteLine($"[ORDER] Email: {userEmail}");
                Console.WriteLine($"[ORDER] Name: {userName}");

                if (string.IsNullOrEmpty(userEmail))
                {
                    return BadRequest(new { message = "User email not found in token." });
                }

                var order = await _orderService.PlaceOrderAsync(
                    userId, userEmail, userName, dto);

                return Ok(order);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("my-orders")]
        public async Task<IActionResult> GetMyOrders()
        {
            var orders = await _orderService.GetMyOrdersAsync(GetUserId());
            return Ok(orders);
        }

        [HttpGet("{orderId:guid}")]
        public async Task<IActionResult> GetOrderById(Guid orderId)
        {
            var order = await _orderService.GetOrderByIdAsync(GetUserId(), orderId);
            if (order is null) return NotFound(new { message = "Order not found." });
            return Ok(order);
        }

        [HttpPut("{orderId:guid}/status")]
        [Authorize(Roles = "Owner,Admin")]
        public async Task<IActionResult> UpdateStatus(Guid orderId, [FromBody] UpdateOrderStatusDto dto)
        {
            var result = await _orderService.UpdateOrderStatusAsync(orderId, dto.Status);
            if (!result) return NotFound(new { message = "Order not found." });
            return Ok(new { message = "Order status updated." });
        }

        [HttpGet("all")]
        [Authorize(Roles = "Owner,Admin")]
        public async Task<IActionResult> GetAllOrders()
        {
            var orders = await _orderService.GetAllOrdersAsync();
            return Ok(orders);
        }
    }
}