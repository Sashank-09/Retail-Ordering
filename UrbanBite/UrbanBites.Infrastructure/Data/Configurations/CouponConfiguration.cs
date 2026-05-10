using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UrbanBites.Domain.Entities;

namespace UrbanBites.Infrastructure.Data.Configurations
{
    public class CouponConfiguration : IEntityTypeConfiguration<Coupon>
    {
        public void Configure(EntityTypeBuilder<Coupon> builder)
        {
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Code).IsRequired().HasMaxLength(30);
            builder.HasIndex(c => c.Code).IsUnique();
            builder.Property(c => c.Description).HasMaxLength(300);
            builder.Property(c => c.DiscountPercent).HasColumnType("decimal(5,2)");
            builder.Property(c => c.MinOrderAmount).HasColumnType("decimal(10,2)");
            builder.HasQueryFilter(c => !c.IsDeleted);
        }
    }
}