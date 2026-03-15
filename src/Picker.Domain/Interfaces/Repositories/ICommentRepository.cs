using Picker.Domain.Models;
using Picker.Domain.Enums;

namespace Picker.Domain.Interfaces.Repositories;

public interface ICommentRepository : IGenericRepository<Comment>
{
    Task<IEnumerable<Comment>> GetByItemAsync(Guid itemId, CategoryType categoryType);
}
