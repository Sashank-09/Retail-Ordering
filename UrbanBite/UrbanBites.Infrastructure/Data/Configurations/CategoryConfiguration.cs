using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UrbanBites.Domain.Entities;

namespace UrbanBites.Infrastructure.Data.Configurations
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasQueryFilter(c => !c.IsDeleted);

            // Brand → Categories (One to Many)
            builder.HasOne(c => c.Brand)
                .WithMany(b => b.Categories)
                .HasForeignKey(c => c.BrandId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}