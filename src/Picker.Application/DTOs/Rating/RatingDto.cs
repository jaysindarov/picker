using Picker.Domain.Enums;

namespace Picker.Application.DTOs.Rating;

public class RatingDto
{
    public Guid Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public CategoryType CategoryType { get; set; }
    public Guid ItemId { get; set; }
    public int Value { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
