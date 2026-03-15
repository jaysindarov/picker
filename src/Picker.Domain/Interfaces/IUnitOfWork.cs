using Picker.Domain.Interfaces.Repositories;

namespace Picker.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IFoodRepository Foods { get; }
    IMovieRepository Movies { get; }
    IBookRepository Books { get; }
    ICuisineRepository Cuisines { get; }
    IGenreRepository Genres { get; }
    ICommentRepository Comments { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
