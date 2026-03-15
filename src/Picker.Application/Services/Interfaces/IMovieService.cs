using Picker.Application.DTOs.Movie;

namespace Picker.Application.Services.Interfaces;

public interface IMovieService
{
    Task<IEnumerable<MovieDto>> GetAllAsync(Guid? genreId = null);
    Task<MovieDto> GetByIdAsync(Guid id);
    Task<MovieDto> GetRandomAsync(Guid? genreId = null);
    Task<MovieDto> CreateAsync(CreateMovieDto dto);
    Task<MovieDto> UpdateAsync(Guid id, UpdateMovieDto dto);
    Task DeleteAsync(Guid id);
}
