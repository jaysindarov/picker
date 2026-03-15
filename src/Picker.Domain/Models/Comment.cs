using Picker.Domain.Enums;

namespace Picker.Domain.Models;

public class Comment : BaseEntity
{
    public string Content { get; set; } = string.Empty;
    public string AuthorName { get; set; } = string.Empty;
    public CategoryType CategoryType { get; set; }
    public Guid ItemId { get; set; }

    public Guid? FoodId { get; set; }
    public Food? Food { get; set; }

    public Guid? MovieId { get; set; }
    public Movie? Movie { get; set; }

    public Guid? BookId { get; set; }
    public Book? Book { get; set; }
}
