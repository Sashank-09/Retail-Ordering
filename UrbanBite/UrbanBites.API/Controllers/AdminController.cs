using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using UrbanBites.Infrastructure.Data;
using UrbanBites.Infrastructure.Identity;
using UrbanBites.Domain.Enums;

namespace UrbanBites.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public AdminController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        private Guid GetCurrentUserId() =>
            Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        private async Task<string> GetHighestRole(AppUser user)
        {
            if (await _userManager.IsInRoleAsync(user, "Owner")) return "Owner";
            if (await _userManager.IsInRoleAsync(user, "Admin")) return "Admin";
            return "Customer";
        }

        // ── Dashboard ─────────────────────────────────────────────────────────
        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboard()
        {
            var totalOrders = await _context.Orders.CountAsync();

            var totalRevenue = await _context.Orders
                                     .Where(o => !o.IsDeleted)
                                     .SumAsync(o => o.TotalAmount);

            var totalProducts = await _context.Products
                                     .Where(p => !p.IsDeleted)
                                     .CountAsync();

            var totalCustomers = (await _userManager.GetUsersInRoleAsync("Customer")).Count;

            var pendingOrders = await _context.Orders
                                     .Where(o => o.Status == OrderStatus.Pending)
                                     .CountAsync();

            var recentOrders = await _context.Orders
                                     .Include(o => o.OrderItems)
                                     .OrderByDescending(o => o.PlacedAt)
                                     .Take(5)
                                     .Select(o => new
                                     {
                                         o.Id,
                                         o.TotalAmount,
                                         o.PlacedAt,
                                         o.Status,
                                         ItemCount = o.OrderItems.Count
                                     })
                                     .ToListAsync();

            var recentOrdersMapped = recentOrders.Select(o => new
            {
                o.Id,
                o.TotalAmount,
                o.PlacedAt,
                Status = o.Status.ToString(),
                o.ItemCount
            });

            return Ok(new
            {
                totalOrders,
                totalRevenue,
                totalProducts,
                totalCustomers,
                pendingOrders,
                recentOrders = recentOrdersMapped
            });
        }

        // ── Get All Users ─────────────────────────────────────────────────────
        [HttpGet("users")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _userManager.Users.ToListAsync();

            var result = new List<object>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var isLockedOut = await _userManager.IsLockedOutAsync(user);
                var highestRole = await GetHighestRole(user);

                result.Add(new
                {
                    user.Id,
                    user.FirstName,
                    user.LastName,
                    user.Email,
                    user.PhoneNumber,
                    Roles = roles,
                    HighestRole = highestRole,
                    IsLockedOut = isLockedOut
                });
            }

            return Ok(result);
        }

        // ── Lock User ─────────────────────────────────────────────────────────
        [HttpPost("users/{id}/lock")]
        public async Task<IActionResult> LockUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound(new { message = "User not found." });

            // Owners cannot be locked
            if (await _userManager.IsInRoleAsync(user, "Owner"))
                return BadRequest(new { message = "Cannot lock an Owner account." });

            // Admins can only be locked by Owners
            if (await _userManager.IsInRoleAsync(user, "Admin"))
            {
                var currentUser = await _userManager.FindByIdAsync(GetCurrentUserId().ToString());
                if (currentUser == null || !await _userManager.IsInRoleAsync(currentUser, "Owner"))
                    return Forbid();
            }

            var result = await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue);
            await _userManager.SetLockoutEnabledAsync(user, true);

            if (!result.Succeeded)
                return BadRequest(new { message = "Failed to lock user." });

            return Ok(new { message = "User locked successfully." });
        }

        // ── Unlock User ───────────────────────────────────────────────────────
        [HttpPost("users/{id}/unlock")]
        public async Task<IActionResult> UnlockUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound(new { message = "User not found." });

            // Admins can only be unlocked by Owners
            if (await _userManager.IsInRoleAsync(user, "Admin"))
            {
                var currentUser = await _userManager.FindByIdAsync(GetCurrentUserId().ToString());
                if (currentUser == null || !await _userManager.IsInRoleAsync(currentUser, "Owner"))
                    return Forbid();
            }

            var result = await _userManager.SetLockoutEndDateAsync(user, null);

            if (!result.Succeeded)
                return BadRequest(new { message = "Failed to unlock user." });

            return Ok(new { message = "User unlocked successfully." });
        }

        // ── Promote to Admin (Owner Only) ─────────────────────────────────────
        [HttpPost("users/{id}/make-admin")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> MakeAdmin(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound(new { message = "User not found." });

            if (await _userManager.IsInRoleAsync(user, "Owner"))
                return BadRequest(new { message = "Cannot change role of an Owner." });

            if (await _userManager.IsInRoleAsync(user, "Admin"))
                return BadRequest(new { message = "User is already an Admin." });

            var result = await _userManager.AddToRoleAsync(user, "Admin");

            if (!result.Succeeded)
                return BadRequest(new { message = "Failed to assign Admin role." });

            return Ok(new { message = "User promoted to Admin." });
        }

        // ── Remove Admin Role (Owner Only) ────────────────────────────────────
        [HttpPost("users/{id}/remove-admin")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RemoveAdmin(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound(new { message = "User not found." });

            if (await _userManager.IsInRoleAsync(user, "Owner"))
                return BadRequest(new { message = "Cannot change role of an Owner." });

            if (!await _userManager.IsInRoleAsync(user, "Admin"))
                return BadRequest(new { message = "User is not an Admin." });

            var result = await _userManager.RemoveFromRoleAsync(user, "Admin");

            if (!result.Succeeded)
                return BadRequest(new { message = "Failed to remove Admin role." });

            return Ok(new { message = "Admin role removed." });
        }

        // ── Delete Account (Owner Only) ───────────────────────────────────────
        [HttpDelete("users/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteAccount(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound(new { message = "User not found." });

            // Cannot delete Owner accounts
            if (await _userManager.IsInRoleAsync(user, "Owner"))
                return BadRequest(new { message = "Cannot delete an Owner account. Transfer ownership first." });

            // Cannot delete yourself
            if (user.Id == GetCurrentUserId())
                return BadRequest(new { message = "Cannot delete your own account." });

            var result = await _userManager.DeleteAsync(user);

            if (!result.Succeeded)
                return BadRequest(new { message = "Failed to delete user account." });

            return Ok(new { message = "User account deleted successfully." });
        }

        // ── Transfer Ownership (Owner Only) ───────────────────────────────────
        [HttpPost("users/{id}/transfer-ownership")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> TransferOwnership(string id)
        {
            var targetUser = await _userManager.FindByIdAsync(id);
            if (targetUser == null) return NotFound(new { message = "User not found." });

            var currentUser = await _userManager.FindByIdAsync(GetCurrentUserId().ToString());
            if (currentUser == null) return Unauthorized();

            if (targetUser.Id == currentUser.Id)
                return BadRequest(new { message = "You already own the platform." });

            // Add Owner role to target
            if (!await _userManager.IsInRoleAsync(targetUser, "Owner"))
                await _userManager.AddToRoleAsync(targetUser, "Owner");

            // Ensure target also has Admin role for fallback
            if (!await _userManager.IsInRoleAsync(targetUser, "Admin"))
                await _userManager.AddToRoleAsync(targetUser, "Admin");

            // Remove Owner role from current user, keep Admin
            await _userManager.RemoveFromRoleAsync(currentUser, "Owner");
            if (!await _userManager.IsInRoleAsync(currentUser, "Admin"))
                await _userManager.AddToRoleAsync(currentUser, "Admin");

            return Ok(new { message = $"Ownership transferred to {targetUser.Email}." });
        }
    }
}