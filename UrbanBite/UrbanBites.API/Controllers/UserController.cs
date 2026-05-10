using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UrbanBites.Application.DTOs.Auth;
using UrbanBites.Infrastructure.Identity;

namespace UrbanBites.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;

        public UserController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        private Guid GetUserId() =>
            Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var user = await _userManager.FindByIdAsync(GetUserId().ToString());
            if (user is null) return NotFound();

            var roles = await _userManager.GetRolesAsync(user);
            return Ok(new UserProfileDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email!,
                Phone = user.PhoneNumber ?? string.Empty,
                Role = roles.FirstOrDefault() ?? "Customer"
            });
        }

        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto dto)
        {
            var user = await _userManager.FindByIdAsync(GetUserId().ToString());
            if (user is null) return NotFound();

            user.FullName = dto.FullName;
            user.PhoneNumber = dto.Phone;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return BadRequest(result.Errors.Select(e => e.Description));

            return Ok(new { message = "Profile updated successfully." });
        }
    }
}