using Picker.Domain.Interfaces;
using Picker.Domain.Interfaces.Repositories;
using Picker.Infrastructure.Data;
using Picker.Infrastructure.Repositories;

namespace Picker.Infrastructure;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    public IFoodRepository Foods { get; }
    public IMovieRepository Movies { get; }
    public IBookRepository Books { get; }
    public ICuisineRepository Cuisines { get; }
    public IGenreRepository Genres { get; }
    public ICommentRepository Comments { get; }
    public IRatingRepository Ratings { get; }

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
        Foods = new FoodRepository(context);
        Movies = new MovieRepository(context);
        Books = new BookRepository(context);
        Cuisines = new CuisineRepository(context);
        Genres = new GenreRepository(context);
        Comments = new CommentRepository(context);
        Ratings = new RatingRepository(context);
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        _context.SaveChangesAsync(cancellationToken);

    public void Dispose() => _context.Dispose();
}
