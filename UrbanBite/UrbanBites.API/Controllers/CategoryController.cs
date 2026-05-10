using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using UrbanBites.Application.DTOs.Category;
using UrbanBites.Application.Interfaces.Services;

namespace UrbanBites.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [EnableRateLimiting("GeneralPolicy")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var categories = await _categoryService.GetAllAsync();
            return Ok(categories);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var category = await _categoryService.GetByIdAsync(id);
            if (category is null) return NotFound(new { message = "Category not found." });
            return Ok(category);
        }

        [HttpGet("by-brand/{brandId:guid}")]
        public async Task<IActionResult> GetByBrand(Guid brandId)
        {
            var categories = await _categoryService.GetByBrandAsync(brandId);
            return Ok(categories);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CreateCategoryDto dto)
        {
            var category = await _categoryService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = category.Id }, category);
        }

        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCategoryDto dto)
        {
            var category = await _categoryService.UpdateAsync(id, dto);
            if (category is null) return NotFound(new { message = "Category not found." });
            return Ok(category);
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _categoryService.DeleteAsync(id);
            if (!result) return NotFound(new { message = "Category not found." });
            return Ok(new { message = "Category deleted successfully." });
        }
    }
}