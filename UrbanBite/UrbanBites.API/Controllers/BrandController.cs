using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using UrbanBites.Application.DTOs.Brand;
using UrbanBites.Application.Interfaces.Services;

namespace UrbanBites.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [EnableRateLimiting("GeneralPolicy")]
    public class BrandController : ControllerBase
    {
        private readonly IBrandService _brandService;

        public BrandController(IBrandService brandService)
        {
            _brandService = brandService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var brands = await _brandService.GetAllAsync();
            return Ok(brands);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var brand = await _brandService.GetByIdAsync(id);
            if (brand is null) return NotFound(new { message = "Brand not found." });
            return Ok(brand);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CreateBrandDto dto)
        {
            var brand = await _brandService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = brand.Id }, brand);
        }

        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateBrandDto dto)
        {
            var brand = await _brandService.UpdateAsync(id, dto);
            if (brand is null) return NotFound(new { message = "Brand not found." });
            return Ok(brand);
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _brandService.DeleteAsync(id);
            if (!result) return NotFound(new { message = "Brand not found." });
            return Ok(new { message = "Brand deleted successfully." });
        }
    }
}