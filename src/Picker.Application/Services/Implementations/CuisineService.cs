using Picker.Application.Common.Exceptions;
using Picker.Application.DTOs.Cuisine;
using Picker.Application.Services.Interfaces;
using Picker.Domain.Entities;
using Picker.Domain.Interfaces;

namespace Picker.Application.Services.Implementations;

public class CuisineService : ICuisineService
{
    private readonly IUnitOfWork _uow;

    public CuisineService(IUnitOfWork uow) => _uow = uow;

    public async Task<IEnumerable<CuisineDto>> GetAllAsync()
    {
        var cuisines = await _uow.Cuisines.GetAllAsync();
        return cuisines.Select(MapToDto);
    }

    public async Task<CuisineDto> GetByIdAsync(Guid id)
    {
        var cuisine = await _uow.Cuisines.GetByIdAsync(id)
            ?? throw new NotFoundException(nameof(Cuisine), id);
        return MapToDto(cuisine);
    }

    public async Task<CuisineDto> CreateAsync(CreateCuisineDto dto)
    {
        var cuisine = new Cuisine { Name = dto.Name };
        await _uow.Cuisines.AddAsync(cuisine);
        await _uow.SaveChangesAsync();
        return MapToDto(cuisine);
    }

    public async Task<CuisineDto> UpdateAsync(Guid id, UpdateCuisineDto dto)
    {
        var cuisine = await _uow.Cuisines.GetByIdAsync(id)
            ?? throw new NotFoundException(nameof(Cuisine), id);
        cuisine.Name = dto.Name;
        cuisine.UpdatedAt = DateTime.UtcNow;
        _uow.Cuisines.Update(cuisine);
        await _uow.SaveChangesAsync();
        return MapToDto(cuisine);
    }

    public async Task DeleteAsync(Guid id)
    {
        var cuisine = await _uow.Cuisines.GetByIdAsync(id)
            ?? throw new NotFoundException(nameof(Cuisine), id);
        _uow.Cuisines.Delete(cuisine);
        await _uow.SaveChangesAsync();
    }

    private static CuisineDto MapToDto(Cuisine c) => new()
    {
        Id = c.Id,
        Name = c.Name,
        CreatedAt = c.CreatedAt,
        UpdatedAt = c.UpdatedAt
    };
}
