namespace Picker.Domain.Entities;

public class Cuisine : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public ICollection<Food> Foods { get; set; } = new List<Food>();
}
