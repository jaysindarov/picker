using Picker.Domain.Entities;

namespace Picker.Domain.Interfaces.Repositories;

public interface IMovieRepository : IGenericRepository<Movie>
{
    Task<IEnumerable<Movie>> GetAllWithDetailsAsync(Guid? genreId = null);
    Task<Movie?> GetByIdWithDetailsAsync(Guid id);
    Task<Movie?> GetRandomAsync(Guid? genreId = null);
}
