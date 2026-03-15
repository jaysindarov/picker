using Microsoft.EntityFrameworkCore;
using Picker.Domain.Entities;
using Picker.Domain.Interfaces.Repositories;
using Picker.Infrastructure.Data;

namespace Picker.Infrastructure.Repositories;

public class BookRepository : GenericRepository<Book>, IBookRepository
{
    public BookRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<Book>> GetAllWithDetailsAsync(Guid? genreId = null)
    {
        var query = _context.Books
            .Include(b => b.Genre)
            .Include(b => b.Comments)
            .AsQueryable();

        if (genreId.HasValue)
            query = query.Where(b => b.GenreId == genreId.Value);

        return await query.ToListAsync();
    }

    public async Task<Book?> GetByIdWithDetailsAsync(Guid id) =>
        await _context.Books
            .Include(b => b.Genre)
            .Include(b => b.Comments)
            .FirstOrDefaultAsync(b => b.Id == id);

    public async Task<Book?> GetRandomAsync(Guid? genreId = null)
    {
        var query = _context.Books
            .Include(b => b.Genre)
            .Include(b => b.Comments)
            .AsQueryable();

        if (genreId.HasValue)
            query = query.Where(b => b.GenreId == genreId.Value);

        var count = await query.CountAsync();
        if (count == 0) return null;

        var skip = Random.Shared.Next(count);
        return await query.Skip(skip).FirstOrDefaultAsync();
    }
}
