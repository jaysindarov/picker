using Microsoft.EntityFrameworkCore;
using Picker.Domain.Entities;

namespace Picker.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Food> Foods => Set<Food>();
    public DbSet<Movie> Movies => Set<Movie>();
    public DbSet<Book> Books => Set<Book>();
    public DbSet<Cuisine> Cuisines => Set<Cuisine>();
    public DbSet<Genre> Genres => Set<Genre>();
    public DbSet<Comment> Comments => Set<Comment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
