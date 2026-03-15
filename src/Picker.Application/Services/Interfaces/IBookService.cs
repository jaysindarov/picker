using Picker.Application.DTOs.Book;

namespace Picker.Application.Services.Interfaces;

public interface IBookService
{
    Task<IEnumerable<BookDto>> GetAllAsync(Guid? genreId = null);
    Task<BookDto> GetByIdAsync(Guid id);
    Task<BookDto> GetRandomAsync(Guid? genreId = null);
    Task<BookDto> CreateAsync(CreateBookDto dto);
    Task<BookDto> UpdateAsync(Guid id, UpdateBookDto dto);
    Task DeleteAsync(Guid id);
}
