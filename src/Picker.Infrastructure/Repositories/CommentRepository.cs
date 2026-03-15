using Microsoft.EntityFrameworkCore;
using Picker.Domain.Models;
using Picker.Domain.Enums;
using Picker.Domain.Interfaces.Repositories;
using Picker.Infrastructure.Data;

namespace Picker.Infrastructure.Repositories;

public class CommentRepository : GenericRepository<Comment>, ICommentRepository
{
    public CommentRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<Comment>> GetByItemAsync(Guid itemId, CategoryType categoryType) =>
        await _context.Comments
            .Where(c => c.ItemId == itemId && c.CategoryType == categoryType)
            .ToListAsync();
}
