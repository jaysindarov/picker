using Picker.Application.DTOs.Rating;
using Picker.Domain.Enums;

namespace Picker.Application.Services.Interfaces;

public interface IRatingService
{
    Task<RatingDto> UpsertAsync(CreateRatingDto dto, string userId);
    Task<RatingDto?> GetUserRatingAsync(Guid itemId, CategoryType categoryType, string userId);
}
