using Picker.Domain.Enums;
using Picker.Domain.Models;

namespace Picker.Domain.Interfaces.Repositories;

public interface IRatingRepository : IGenericRepository<Rating>
{
    Task<Rating?> GetByUserAndItemAsync(string userId, Guid itemId, CategoryType categoryType);
    Task<double?> GetAverageAsync(Guid itemId, CategoryType categoryType);
    Task<int> GetCountAsync(Guid itemId, CategoryType categoryType);
}
