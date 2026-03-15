using Picker.Domain.Entities;

namespace Picker.Domain.Interfaces.Repositories;

public interface IBookRepository : IGenericRepository<Book>
{
    Task<IEnumerable<Book>> GetAllWithDetailsAsync(Guid? genreId = null);
    Task<Book?> GetByIdWithDetailsAsync(Guid id);
    Task<Book?> GetRandomAsync(Guid? genreId = null);
}
