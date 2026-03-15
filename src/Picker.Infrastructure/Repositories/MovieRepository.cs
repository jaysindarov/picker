using Microsoft.EntityFrameworkCore;
using Picker.Domain.Entities;
using Picker.Domain.Interfaces.Repositories;
using Picker.Infrastructure.Data;

namespace Picker.Infrastructure.Repositories;

public class MovieRepository : GenericRepository<Movie>, IMovieRepository
{
    public MovieRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<Movie>> GetAllWithDetailsAsync(Guid? genreId = null)
    {
        var query = _context.Movies
            .Include(m => m.Genre)
            .Include(m => m.Comments)
            .AsQueryable();

        if (genreId.HasValue)
            query = query.Where(m => m.GenreId == genreId.Value);

        return await query.ToListAsync();
    }

    public async Task<Movie?> GetByIdWithDetailsAsync(Guid id) =>
        await _context.Movies
            .Include(m => m.Genre)
            .Include(m => m.Comments)
            .FirstOrDefaultAsync(m => m.Id == id);

    public async Task<Movie?> GetRandomAsync(Guid? genreId = null)
    {
        var query = _context.Movies
            .Include(m => m.Genre)
            .Include(m => m.Comments)
            .AsQueryable();

        if (genreId.HasValue)
            query = query.Where(m => m.GenreId == genreId.Value);

        var count = await query.CountAsync();
        if (count == 0) return null;

        var skip = Random.Shared.Next(count);
        return await query.Skip(skip).FirstOrDefaultAsync();
    }
}
