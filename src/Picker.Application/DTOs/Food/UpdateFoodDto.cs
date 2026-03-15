namespace Picker.Application.DTOs.Food;

public class UpdateFoodDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public Guid CuisineId { get; set; }
}
