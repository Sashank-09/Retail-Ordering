using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UrbanBites.Domain.Entities;
using UrbanBites.Infrastructure.Identity;

namespace UrbanBites.Infrastructure.Data.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.HasKey(o => o.Id);

            builder.Property(o => o.TotalAmount)
                .HasColumnType("decimal(10,2)");

            builder.Property(o => o.DeliveryAddress)
                .IsRequired()
                .HasMaxLength(300);

            // Store enum as string in DB — readable in SSMS
            builder.Property(o => o.Status)
                .HasConversion<string>()
                .HasMaxLength(30);

            builder.HasQueryFilter(o => !o.IsDeleted);

            // User → Orders (One to Many)
            builder.HasOne<AppUser>()
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}