using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Picker.Domain.Models;

namespace Picker.Infrastructure.Data.Configurations;

public class RatingConfiguration : IEntityTypeConfiguration<Rating>
{
    public void Configure(EntityTypeBuilder<Rating> builder)
    {
        builder.HasKey(r => r.Id);
        builder.Property(r => r.UserId).IsRequired().HasMaxLength(450);
        builder.Property(r => r.Value).IsRequired();

        builder.HasIndex(r => new { r.UserId, r.ItemId, r.CategoryType }).IsUnique();

        builder.HasOne(r => r.Food)
            .WithMany(f => f.Ratings)
            .HasForeignKey(r => r.FoodId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(r => r.Movie)
            .WithMany(m => m.Ratings)
            .HasForeignKey(r => r.MovieId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(r => r.Book)
            .WithMany(b => b.Ratings)
            .HasForeignKey(r => r.BookId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
