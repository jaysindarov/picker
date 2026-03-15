using Picker.Domain.Entities;
using Picker.Domain.Interfaces.Repositories;
using Picker.Infrastructure.Data;

namespace Picker.Infrastructure.Repositories;

public class CuisineRepository : GenericRepository<Cuisine>, ICuisineRepository
{
    public CuisineRepository(AppDbContext context) : base(context) { }
}
