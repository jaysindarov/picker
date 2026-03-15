using Picker.Domain.Entities;
using Picker.Domain.Interfaces.Repositories;
using Picker.Infrastructure.Data;

namespace Picker.Infrastructure.Repositories;

public class GenreRepository : GenericRepository<Genre>, IGenreRepository
{
    public GenreRepository(AppDbContext context) : base(context) { }
}
