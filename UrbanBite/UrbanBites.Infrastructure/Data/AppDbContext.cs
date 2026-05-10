using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UrbanBites.Domain.Entities;
using UrbanBites.Infrastructure.Identity;

namespace UrbanBites.Infrastructure.Data
{
    public class AppDbContext : IdentityDbContext<AppUser, AppRole, Guid>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // Our Tables
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Promotion> Promotions { get; set; }
        public DbSet<Coupon> Coupons { get; set; }
        public DbSet<LoyaltyPoint> LoyaltyPoints { get; set; }
        public DbSet<OtpRecord> OtpRecords { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder); // MUST call this — sets up Identity tables

            // Apply all entity configurations from this assembly
            builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

            // Rename Identity tables to cleaner names
            builder.Entity<AppUser>().ToTable("Users");
            builder.Entity<AppRole>().ToTable("Roles");
        }
    }
}