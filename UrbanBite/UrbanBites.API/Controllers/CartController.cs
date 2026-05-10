using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;
using UrbanBites.Application.DTOs.Cart;
using UrbanBites.Application.Interfaces.Services;

namespace UrbanBites.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    [EnableRateLimiting("GeneralPolicy")]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        private Guid GetUserId() =>
            Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            var cart = await _cartService.GetCartAsync(GetUserId());
            return Ok(cart);
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartDto dto)
        {
            try
            {
                var cart = await _cartService.AddToCartAsync(GetUserId(), dto);
                return Ok(cart);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("update/{cartItemId:guid}")]
        public async Task<IActionResult> UpdateItem(Guid cartItemId,
                                                     [FromBody] UpdateCartItemDto dto)
        {
            try
            {
                var cart = await _cartService.UpdateCartItemAsync(
                    GetUserId(), cartItemId, dto);
                return Ok(cart);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("remove/{cartItemId:guid}")]
        public async Task<IActionResult> RemoveItem(Guid cartItemId)
        {
            var result = await _cartService.RemoveCartItemAsync(GetUserId(), cartItemId);
            if (!result) return NotFound(new { message = "Cart item not found." });
            return Ok(new { message = "Item removed from cart." });
        }

        [HttpDelete("clear")]
        public async Task<IActionResult> ClearCart()
        {
            await _cartService.ClearCartAsync(GetUserId());
            return Ok(new { message = "Cart cleared." });
        }
    }
}