using Microsoft.EntityFrameworkCore;
using Picker.Domain.Enums;
using Picker.Domain.Interfaces.Repositories;
using Picker.Domain.Models;
using Picker.Infrastructure.Data;

namespace Picker.Infrastructure.Repositories;

public class RatingRepository : GenericRepository<Rating>, IRatingRepository
{
    public RatingRepository(AppDbContext context) : base(context) { }

    public async Task<Rating?> GetByUserAndItemAsync(string userId, Guid itemId, CategoryType categoryType) =>
        await _context.Ratings
            .FirstOrDefaultAsync(r => r.UserId == userId && r.ItemId == itemId && r.CategoryType == categoryType);

    public async Task<double?> GetAverageAsync(Guid itemId, CategoryType categoryType) =>
        await _context.Ratings
            .Where(r => r.ItemId == itemId && r.CategoryType == categoryType)
            .AverageAsync(r => (double?)r.Value);

    public async Task<int> GetCountAsync(Guid itemId, CategoryType categoryType) =>
        await _context.Ratings
            .CountAsync(r => r.ItemId == itemId && r.CategoryType == categoryType);
}
