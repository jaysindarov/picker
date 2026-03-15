using Picker.Application.Common.Exceptions;
using Picker.Application.DTOs.Comment;
using Picker.Application.Services.Interfaces;
using Picker.Domain.Entities;
using Picker.Domain.Enums;
using Picker.Domain.Interfaces;

namespace Picker.Application.Services.Implementations;

public class CommentService : ICommentService
{
    private readonly IUnitOfWork _uow;

    public CommentService(IUnitOfWork uow) => _uow = uow;

    public async Task<IEnumerable<CommentDto>> GetByItemAsync(Guid itemId, CategoryType categoryType)
    {
        var comments = await _uow.Comments.GetByItemAsync(itemId, categoryType);
        return comments.Select(MapToDto);
    }

    public async Task<CommentDto> GetByIdAsync(Guid id)
    {
        var comment = await _uow.Comments.GetByIdAsync(id)
            ?? throw new NotFoundException(nameof(Comment), id);
        return MapToDto(comment);
    }

    public async Task<CommentDto> CreateAsync(CreateCommentDto dto)
    {
        var comment = new Comment
        {
            Content = dto.Content,
            AuthorName = dto.AuthorName,
            CategoryType = dto.CategoryType,
            ItemId = dto.ItemId
        };

        switch (dto.CategoryType)
        {
            case CategoryType.Food:
                _ = await _uow.Foods.GetByIdAsync(dto.ItemId)
                    ?? throw new NotFoundException(nameof(Food), dto.ItemId);
                comment.FoodId = dto.ItemId;
                break;
            case CategoryType.Movie:
                _ = await _uow.Movies.GetByIdAsync(dto.ItemId)
                    ?? throw new NotFoundException(nameof(Movie), dto.ItemId);
                comment.MovieId = dto.ItemId;
                break;
            case CategoryType.Book:
                _ = await _uow.Books.GetByIdAsync(dto.ItemId)
                    ?? throw new NotFoundException(nameof(Book), dto.ItemId);
                comment.BookId = dto.ItemId;
                break;
        }

        await _uow.Comments.AddAsync(comment);
        await _uow.SaveChangesAsync();
        return MapToDto(comment);
    }

    public async Task DeleteAsync(Guid id)
    {
        var comment = await _uow.Comments.GetByIdAsync(id)
            ?? throw new NotFoundException(nameof(Comment), id);
        _uow.Comments.Delete(comment);
        await _uow.SaveChangesAsync();
    }

    private static CommentDto MapToDto(Comment c) => new()
    {
        Id = c.Id,
        Content = c.Content,
        AuthorName = c.AuthorName,
        CategoryType = c.CategoryType,
        ItemId = c.ItemId,
        CreatedAt = c.CreatedAt,
        UpdatedAt = c.UpdatedAt
    };
}
