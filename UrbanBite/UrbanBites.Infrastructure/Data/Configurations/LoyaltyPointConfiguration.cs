using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UrbanBites.Domain.Entities;
using UrbanBites.Infrastructure.Identity;

namespace UrbanBites.Infrastructure.Data.Configurations
{
    public class LoyaltyPointConfiguration : IEntityTypeConfiguration<LoyaltyPoint>
    {
        public void Configure(EntityTypeBuilder<LoyaltyPoint> builder)
        {
            builder.HasKey(l => l.Id);
            builder.Property(l => l.Reason).HasMaxLength(200);
            builder.Property(l => l.Type).HasMaxLength(20);
            builder.HasOne<AppUser>()
                .WithMany()
                .HasForeignKey(l => l.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}