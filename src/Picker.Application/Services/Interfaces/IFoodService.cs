using Picker.Application.DTOs.Food;

namespace Picker.Application.Services.Interfaces;

public interface IFoodService
{
    Task<IEnumerable<FoodDto>> GetAllAsync(Guid? cuisineId = null);
    Task<FoodDto> GetByIdAsync(Guid id);
    Task<FoodDto> GetRandomAsync(Guid? cuisineId = null);
    Task<FoodDto> CreateAsync(CreateFoodDto dto);
    Task<FoodDto> UpdateAsync(Guid id, UpdateFoodDto dto);
    Task DeleteAsync(Guid id);
}
