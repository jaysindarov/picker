using Picker.Domain.Enums;

namespace Picker.Application.DTOs.Comment;

public class CreateCommentDto
{
    public string Content { get; set; } = string.Empty;
    public string AuthorName { get; set; } = string.Empty;
    public CategoryType CategoryType { get; set; }
    public Guid ItemId { get; set; }
}
