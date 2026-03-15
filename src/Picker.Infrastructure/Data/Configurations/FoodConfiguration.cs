using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Picker.Domain.Models;

namespace Picker.Infrastructure.Data.Configurations;

public class FoodConfiguration : IEntityTypeConfiguration<Food>
{
    public void Configure(EntityTypeBuilder<Food> builder)
    {
        builder.HasKey(f => f.Id);
        builder.Property(f => f.Title).IsRequired().HasMaxLength(200);
        builder.Property(f => f.Description).HasMaxLength(2000);
        builder.Property(f => f.ImageUrl).HasMaxLength(500);

        builder.HasOne(f => f.Cuisine)
            .WithMany(c => c.Foods)
            .HasForeignKey(f => f.CuisineId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
