using Picker.Application.DTOs.Comment;

namespace Picker.Application.DTOs.Food;

public class FoodDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public Guid CuisineId { get; set; }
    public string CuisineName { get; set; } = string.Empty;
    public List<CommentDto> Comments { get; set; } = new();
    public double? AverageRating { get; set; }
    public int TotalRatings { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
