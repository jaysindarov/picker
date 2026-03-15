using Picker.Domain.Models;

namespace Picker.Domain.Interfaces.Repositories;

public interface IFoodRepository : IGenericRepository<Food>
{
    Task<IEnumerable<Food>> GetAllWithDetailsAsync(Guid? cuisineId = null);
    Task<Food?> GetByIdWithDetailsAsync(Guid id);
    Task<Food?> GetRandomAsync(Guid? cuisineId = null);
}
