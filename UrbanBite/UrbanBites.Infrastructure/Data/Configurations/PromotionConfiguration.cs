using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UrbanBites.Domain.Entities;

namespace UrbanBites.Infrastructure.Data.Configurations
{
    public class PromotionConfiguration : IEntityTypeConfiguration<Promotion>
    {
        public void Configure(EntityTypeBuilder<Promotion> builder)
        {
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Title).IsRequired().HasMaxLength(150);
            builder.Property(p => p.Description).HasMaxLength(500);
            builder.Property(p => p.DiscountPercent).HasColumnType("decimal(5,2)");
            builder.Property(p => p.ImageUrl).HasMaxLength(500);
            builder.HasQueryFilter(p => !p.IsDeleted);
        }
    }
}