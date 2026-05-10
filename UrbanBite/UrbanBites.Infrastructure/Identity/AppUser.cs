using Microsoft.AspNetCore.Identity;
using UrbanBites.Domain.Entities;

namespace UrbanBites.Infrastructure.Identity
{
    public class AppUser : IdentityUser<Guid>
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public bool IsDeleted { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public Cart? Cart { get; set; }
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}