using Microsoft.AspNetCore.Identity;
using UrbanBites.Application.Interfaces.Repositories;
using UrbanBites.Infrastructure.Identity;

namespace UrbanBites.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<AppUser> _userManager;

        public UserRepository(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<string?> GetUserEmailAsync(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            return user?.Email;
        }

        public async Task<string?> GetUserNameAsync(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            return $"{user?.FirstName} {user?.LastName}".Trim();
        }
    }
}
