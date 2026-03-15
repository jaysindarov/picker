using Picker.Application.Common.Exceptions;
using Picker.Application.DTOs.Book;
using Picker.Application.DTOs.Comment;
using Picker.Application.Services.Interfaces;
using Picker.Domain.Models;
using Picker.Domain.Interfaces;

namespace Picker.Application.Services.Implementations;

public class BookService : IBookService
{
    private readonly IUnitOfWork _uow;

    public BookService(IUnitOfWork uow) => _uow = uow;

    public async Task<IEnumerable<BookDto>> GetAllAsync(Guid? genreId = null)
    {
        var books = await _uow.Books.GetAllWithDetailsAsync(genreId);
        return books.Select(MapToDto);
    }

    public async Task<BookDto> GetByIdAsync(Guid id)
    {
        var book = await _uow.Books.GetByIdWithDetailsAsync(id)
            ?? throw new NotFoundException(nameof(Book), id);
        return MapToDto(book);
    }

    public async Task<BookDto> GetRandomAsync(Guid? genreId = null)
    {
        var book = await _uow.Books.GetRandomAsync(genreId)
            ?? throw new NotFoundException(nameof(Book), "random");
        return MapToDto(book);
    }

    public async Task<BookDto> CreateAsync(CreateBookDto dto)
    {
        _ = await _uow.Genres.GetByIdAsync(dto.GenreId)
            ?? throw new NotFoundException(nameof(Genre), dto.GenreId);

        var book = new Book
        {
            Title = dto.Title,
            Description = dto.Description,
            ImageUrl = dto.ImageUrl,
            GenreId = dto.GenreId
        };
        await _uow.Books.AddAsync(book);
        await _uow.SaveChangesAsync();

        var created = await _uow.Books.GetByIdWithDetailsAsync(book.Id);
        return MapToDto(created!);
    }

    public async Task<BookDto> UpdateAsync(Guid id, UpdateBookDto dto)
    {
        var book = await _uow.Books.GetByIdWithDetailsAsync(id)
            ?? throw new NotFoundException(nameof(Book), id);

        _ = await _uow.Genres.GetByIdAsync(dto.GenreId)
            ?? throw new NotFoundException(nameof(Genre), dto.GenreId);

        book.Title = dto.Title;
        book.Description = dto.Description;
        book.ImageUrl = dto.ImageUrl;
        book.GenreId = dto.GenreId;
        book.UpdatedAt = DateTime.UtcNow;

        _uow.Books.Update(book);
        await _uow.SaveChangesAsync();
        return MapToDto(book);
    }

    public async Task DeleteAsync(Guid id)
    {
        var book = await _uow.Books.GetByIdAsync(id)
            ?? throw new NotFoundException(nameof(Book), id);
        _uow.Books.Delete(book);
        await _uow.SaveChangesAsync();
    }

    private static BookDto MapToDto(Book b) => new()
    {
        Id = b.Id,
        Title = b.Title,
        Description = b.Description,
        ImageUrl = b.ImageUrl,
        GenreId = b.GenreId,
        GenreName = b.Genre?.Name ?? string.Empty,
        Comments = b.Comments.Select(c => new CommentDto
        {
            Id = c.Id,
            Content = c.Content,
            AuthorName = c.AuthorName,
            UserId = c.UserId,
            CategoryType = c.CategoryType,
            ItemId = c.ItemId,
            CreatedAt = c.CreatedAt,
            UpdatedAt = c.UpdatedAt
        }).ToList(),
        AverageRating = b.Ratings.Count > 0 ? Math.Round(b.Ratings.Average(r => (double)r.Value), 2) : null,
        TotalRatings = b.Ratings.Count,
        CreatedAt = b.CreatedAt,
        UpdatedAt = b.UpdatedAt
    };
}
