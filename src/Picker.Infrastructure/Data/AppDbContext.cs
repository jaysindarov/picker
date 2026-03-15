using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Picker.Domain.Models;
using Picker.Infrastructure.Identity;

namespace Picker.Infrastructure.Data;

public class AppDbContext : IdentityDbContext<AppUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Food> Foods => Set<Food>();
    public DbSet<Movie> Movies => Set<Movie>();
    public DbSet<Book> Books => Set<Book>();
    public DbSet<Cuisine> Cuisines => Set<Cuisine>();
    public DbSet<Genre> Genres => Set<Genre>();
    public DbSet<Comment> Comments => Set<Comment>();
    public DbSet<Rating> Ratings => Set<Rating>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder); // Identity tables first
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
