using Microsoft.EntityFrameworkCore;
using Picker.Domain.Entities;
using Picker.Domain.Interfaces.Repositories;
using Picker.Infrastructure.Data;

namespace Picker.Infrastructure.Repositories;

public class FoodRepository : GenericRepository<Food>, IFoodRepository
{
    public FoodRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<Food>> GetAllWithDetailsAsync(Guid? cuisineId = null)
    {
        var query = _context.Foods
            .Include(f => f.Cuisine)
            .Include(f => f.Comments)
            .AsQueryable();

        if (cuisineId.HasValue)
            query = query.Where(f => f.CuisineId == cuisineId.Value);

        return await query.ToListAsync();
    }

    public async Task<Food?> GetByIdWithDetailsAsync(Guid id) =>
        await _context.Foods
            .Include(f => f.Cuisine)
            .Include(f => f.Comments)
            .FirstOrDefaultAsync(f => f.Id == id);

    public async Task<Food?> GetRandomAsync(Guid? cuisineId = null)
    {
        var query = _context.Foods
            .Include(f => f.Cuisine)
            .Include(f => f.Comments)
            .AsQueryable();

        if (cuisineId.HasValue)
            query = query.Where(f => f.CuisineId == cuisineId.Value);

        var count = await query.CountAsync();
        if (count == 0) return null;

        var skip = Random.Shared.Next(count);
        return await query.Skip(skip).FirstOrDefaultAsync();
    }
}
