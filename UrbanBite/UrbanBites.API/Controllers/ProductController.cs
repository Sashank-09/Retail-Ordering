using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using UrbanBites.Application.DTOs.Product;
using UrbanBites.Application.Interfaces.Services;

namespace UrbanBites.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [EnableRateLimiting("GeneralPolicy")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var products = await _productService.GetAllAsync();
            return Ok(products);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var product = await _productService.GetByIdAsync(id);
            if (product is null) return NotFound(new { message = "Product not found." });
            return Ok(product);
        }

        [HttpGet("by-category/{categoryId:guid}")]
        public async Task<IActionResult> GetByCategory(Guid categoryId)
        {
            var products = await _productService.GetByCategoryAsync(categoryId);
            return Ok(products);
        }

        [HttpGet("by-brand/{brandId:guid}")]
        public async Task<IActionResult> GetByBrand(Guid brandId)
        {
            var products = await _productService.GetByBrandAsync(brandId);
            return Ok(products);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CreateProductDto dto)
        {
            var product = await _productService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
        }

        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProductDto dto)
        {
            var product = await _productService.UpdateAsync(id, dto);
            if (product is null) return NotFound(new { message = "Product not found." });
            return Ok(product);
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _productService.DeleteAsync(id);
            if (!result) return NotFound(new { message = "Product not found." });
            return Ok(new { message = "Product deleted successfully." });
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string q)
        {
            var products = await _productService.SearchAsync(q);
            return Ok(products);
        }
    }
}