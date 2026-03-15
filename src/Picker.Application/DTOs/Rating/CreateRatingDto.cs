using Picker.Domain.Enums;

namespace Picker.Application.DTOs.Rating;

public class CreateRatingDto
{
    public CategoryType CategoryType { get; set; }
    public Guid ItemId { get; set; }
    public int Value { get; set; } // 1–5
}
