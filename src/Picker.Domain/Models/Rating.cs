using Picker.Domain.Enums;

namespace Picker.Domain.Models;

public class Rating : BaseEntity
{
    public string UserId { get; set; } = string.Empty;
    public CategoryType CategoryType { get; set; }
    public Guid ItemId { get; set; }
    public int Value { get; set; } // 1–5

    public Guid? FoodId { get; set; }
    public Food? Food { get; set; }

    public Guid? MovieId { get; set; }
    public Movie? Movie { get; set; }

    public Guid? BookId { get; set; }
    public Book? Book { get; set; }
}
