using Picker.Application.DTOs.Cuisine;

namespace Picker.Application.Services.Interfaces;

public interface ICuisineService
{
    Task<IEnumerable<CuisineDto>> GetAllAsync();
    Task<CuisineDto> GetByIdAsync(Guid id);
    Task<CuisineDto> CreateAsync(CreateCuisineDto dto);
    Task<CuisineDto> UpdateAsync(Guid id, UpdateCuisineDto dto);
    Task DeleteAsync(Guid id);
}
