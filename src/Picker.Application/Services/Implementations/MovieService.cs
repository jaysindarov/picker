using Picker.Application.Common.Exceptions;
using Picker.Application.DTOs.Comment;
using Picker.Application.DTOs.Movie;
using Picker.Application.Services.Interfaces;
using Picker.Domain.Entities;
using Picker.Domain.Interfaces;

namespace Picker.Application.Services.Implementations;

public class MovieService : IMovieService
{
    private readonly IUnitOfWork _uow;

    public MovieService(IUnitOfWork uow) => _uow = uow;

    public async Task<IEnumerable<MovieDto>> GetAllAsync(Guid? genreId = null)
    {
        var movies = await _uow.Movies.GetAllWithDetailsAsync(genreId);
        return movies.Select(MapToDto);
    }

    public async Task<MovieDto> GetByIdAsync(Guid id)
    {
        var movie = await _uow.Movies.GetByIdWithDetailsAsync(id)
            ?? throw new NotFoundException(nameof(Movie), id);
        return MapToDto(movie);
    }

    public async Task<MovieDto> GetRandomAsync(Guid? genreId = null)
    {
        var movie = await _uow.Movies.GetRandomAsync(genreId)
            ?? throw new NotFoundException(nameof(Movie), "random");
        return MapToDto(movie);
    }

    public async Task<MovieDto> CreateAsync(CreateMovieDto dto)
    {
        _ = await _uow.Genres.GetByIdAsync(dto.GenreId)
            ?? throw new NotFoundException(nameof(Genre), dto.GenreId);

        var movie = new Movie
        {
            Title = dto.Title,
            Description = dto.Description,
            ImageUrl = dto.ImageUrl,
            GenreId = dto.GenreId
        };
        await _uow.Movies.AddAsync(movie);
        await _uow.SaveChangesAsync();

        var created = await _uow.Movies.GetByIdWithDetailsAsync(movie.Id);
        return MapToDto(created!);
    }

    public async Task<MovieDto> UpdateAsync(Guid id, UpdateMovieDto dto)
    {
        var movie = await _uow.Movies.GetByIdWithDetailsAsync(id)
            ?? throw new NotFoundException(nameof(Movie), id);

        _ = await _uow.Genres.GetByIdAsync(dto.GenreId)
            ?? throw new NotFoundException(nameof(Genre), dto.GenreId);

        movie.Title = dto.Title;
        movie.Description = dto.Description;
        movie.ImageUrl = dto.ImageUrl;
        movie.GenreId = dto.GenreId;
        movie.UpdatedAt = DateTime.UtcNow;

        _uow.Movies.Update(movie);
        await _uow.SaveChangesAsync();
        return MapToDto(movie);
    }

    public async Task DeleteAsync(Guid id)
    {
        var movie = await _uow.Movies.GetByIdAsync(id)
            ?? throw new NotFoundException(nameof(Movie), id);
        _uow.Movies.Delete(movie);
        await _uow.SaveChangesAsync();
    }

    private static MovieDto MapToDto(Movie m) => new()
    {
        Id = m.Id,
        Title = m.Title,
        Description = m.Description,
        ImageUrl = m.ImageUrl,
        GenreId = m.GenreId,
        GenreName = m.Genre?.Name ?? string.Empty,
        Comments = m.Comments.Select(c => new CommentDto
        {
            Id = c.Id,
            Content = c.Content,
            AuthorName = c.AuthorName,
            CategoryType = c.CategoryType,
            ItemId = c.ItemId,
            CreatedAt = c.CreatedAt,
            UpdatedAt = c.UpdatedAt
        }).ToList(),
        CreatedAt = m.CreatedAt,
        UpdatedAt = m.UpdatedAt
    };
}
