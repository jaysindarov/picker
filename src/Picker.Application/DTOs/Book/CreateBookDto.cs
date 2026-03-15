namespace Picker.Application.DTOs.Book;

public class CreateBookDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public Guid GenreId { get; set; }
}
