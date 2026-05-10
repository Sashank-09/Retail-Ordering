using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UrbanBites.Domain.Entities;

namespace UrbanBites.Infrastructure.Data.Configurations
{
    public class OtpRecordConfiguration : IEntityTypeConfiguration<OtpRecord>
    {
        public void Configure(EntityTypeBuilder<OtpRecord> builder)
        {
            builder.HasKey(o => o.Id);
            builder.Property(o => o.Email).IsRequired().HasMaxLength(150);
            builder.Property(o => o.OtpCode).IsRequired().HasMaxLength(10);
        }
    }
}