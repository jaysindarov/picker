using Picker.Application.DTOs.Genre;

namespace Picker.Application.Services.Interfaces;

public interface IGenreService
{
    Task<IEnumerable<GenreDto>> GetAllAsync();
    Task<GenreDto> GetByIdAsync(Guid id);
    Task<GenreDto> CreateAsync(CreateGenreDto dto);
    Task<GenreDto> UpdateAsync(Guid id, UpdateGenreDto dto);
    Task DeleteAsync(Guid id);
}
