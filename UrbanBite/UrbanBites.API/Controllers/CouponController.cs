using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UrbanBites.Application.DTOs.Coupon;
using UrbanBites.Application.Interfaces.Services;

namespace UrbanBites.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CouponController : ControllerBase
    {
        private readonly ICouponService _couponService;

        public CouponController(ICouponService couponService)
        {
            _couponService = couponService;
        }

        [HttpPost("apply")]
        [Authorize]
        public async Task<IActionResult> Apply([FromBody] ApplyCouponDto dto)
        {
            try
            {
                var result = await _couponService.ApplyAsync(dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            var coupons = await _couponService.GetAllAsync();
            return Ok(coupons);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CouponDto dto)
        {
            var coupon = await _couponService.CreateAsync(dto);
            return Ok(coupon);
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _couponService.DeleteAsync(id);
            if (!result) return NotFound();
            return Ok(new { message = "Coupon deleted." });
        }
    }
}