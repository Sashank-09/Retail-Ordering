using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UrbanBites.Application.DTOs.Promotion;
using UrbanBites.Application.Interfaces.Services;

namespace UrbanBites.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PromotionController : ControllerBase
    {
        private readonly IPromotionService _promotionService;

        public PromotionController(IPromotionService promotionService)
        {
            _promotionService = promotionService;
        }

        [HttpGet]
        public async Task<IActionResult> GetActive()
        {
            var promos = await _promotionService.GetActivePromotionsAsync();
            return Ok(promos);
        }

        [HttpGet("all")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            var promos = await _promotionService.GetAllAsync();
            return Ok(promos);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] PromotionDto dto)
        {
            var promo = await _promotionService.CreateAsync(dto);
            return Ok(promo);
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _promotionService.DeleteAsync(id);
            if (!result) return NotFound();
            return Ok(new { message = "Promotion deleted." });
        }
    }
}