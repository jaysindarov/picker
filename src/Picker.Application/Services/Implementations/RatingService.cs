using Picker.Application.Common.Exceptions;
using Picker.Application.DTOs.Rating;
using Picker.Application.Services.Interfaces;
using Picker.Domain.Enums;
using Picker.Domain.Interfaces;
using Picker.Domain.Models;

namespace Picker.Application.Services.Implementations;

public class RatingService : IRatingService
{
    private readonly IUnitOfWork _uow;

    public RatingService(IUnitOfWork uow) => _uow = uow;

    public async Task<RatingDto> UpsertAsync(CreateRatingDto dto, string userId)
    {
        if (dto.Value < 1 || dto.Value > 5)
            throw new BadRequestException("Rating value must be between 1 and 5.");

        var existing = await _uow.Ratings.GetByUserAndItemAsync(userId, dto.ItemId, dto.CategoryType);

        if (existing != null)
        {
            existing.Value = dto.Value;
            existing.UpdatedAt = DateTime.UtcNow;
            _uow.Ratings.Update(existing);
            await _uow.SaveChangesAsync();
            return MapToDto(existing);
        }

        var rating = new Rating
        {
            UserId = userId,
            CategoryType = dto.CategoryType,
            ItemId = dto.ItemId,
            Value = dto.Value
        };

        switch (dto.CategoryType)
        {
            case CategoryType.Food:
                _ = await _uow.Foods.GetByIdAsync(dto.ItemId)
                    ?? throw new NotFoundException(nameof(Food), dto.ItemId);
                rating.FoodId = dto.ItemId;
                break;
            case CategoryType.Movie:
                _ = await _uow.Movies.GetByIdAsync(dto.ItemId)
                    ?? throw new NotFoundException(nameof(Movie), dto.ItemId);
                rating.MovieId = dto.ItemId;
                break;
            case CategoryType.Book:
                _ = await _uow.Books.GetByIdAsync(dto.ItemId)
                    ?? throw new NotFoundException(nameof(Book), dto.ItemId);
                rating.BookId = dto.ItemId;
                break;
        }

        await _uow.Ratings.AddAsync(rating);
        await _uow.SaveChangesAsync();
        return MapToDto(rating);
    }

    public async Task<RatingDto?> GetUserRatingAsync(Guid itemId, CategoryType categoryType, string userId)
    {
        var rating = await _uow.Ratings.GetByUserAndItemAsync(userId, itemId, categoryType);
        return rating is null ? null : MapToDto(rating);
    }

    private static RatingDto MapToDto(Rating r) => new()
    {
        Id = r.Id,
        UserId = r.UserId,
        CategoryType = r.CategoryType,
        ItemId = r.ItemId,
        Value = r.Value,
        CreatedAt = r.CreatedAt,
        UpdatedAt = r.UpdatedAt
    };
}
