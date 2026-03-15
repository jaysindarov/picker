using Picker.Application.Common.Exceptions;
using Picker.Application.DTOs.Genre;
using Picker.Application.Services.Interfaces;
using Picker.Domain.Models;
using Picker.Domain.Interfaces;

namespace Picker.Application.Services.Implementations;

public class GenreService : IGenreService
{
    private readonly IUnitOfWork _uow;

    public GenreService(IUnitOfWork uow) => _uow = uow;

    public async Task<IEnumerable<GenreDto>> GetAllAsync()
    {
        var genres = await _uow.Genres.GetAllAsync();
        return genres.Select(MapToDto);
    }

    public async Task<GenreDto> GetByIdAsync(Guid id)
    {
        var genre = await _uow.Genres.GetByIdAsync(id)
            ?? throw new NotFoundException(nameof(Genre), id);
        return MapToDto(genre);
    }

    public async Task<GenreDto> CreateAsync(CreateGenreDto dto)
    {
        var genre = new Genre { Name = dto.Name };
        await _uow.Genres.AddAsync(genre);
        await _uow.SaveChangesAsync();
        return MapToDto(genre);
    }

    public async Task<GenreDto> UpdateAsync(Guid id, UpdateGenreDto dto)
    {
        var genre = await _uow.Genres.GetByIdAsync(id)
            ?? throw new NotFoundException(nameof(Genre), id);
        genre.Name = dto.Name;
        genre.UpdatedAt = DateTime.UtcNow;
        _uow.Genres.Update(genre);
        await _uow.SaveChangesAsync();
        return MapToDto(genre);
    }

    public async Task DeleteAsync(Guid id)
    {
        var genre = await _uow.Genres.GetByIdAsync(id)
            ?? throw new NotFoundException(nameof(Genre), id);
        _uow.Genres.Delete(genre);
        await _uow.SaveChangesAsync();
    }

    private static GenreDto MapToDto(Genre g) => new()
    {
        Id = g.Id,
        Name = g.Name,
        CreatedAt = g.CreatedAt,
        UpdatedAt = g.UpdatedAt
    };
}
