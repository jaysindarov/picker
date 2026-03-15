using Picker.Application.DTOs.Comment;

namespace Picker.Application.DTOs.Movie;

public class MovieDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public Guid GenreId { get; set; }
    public string GenreName { get; set; } = string.Empty;
    public List<CommentDto> Comments { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
