using Picker.Application.Common.Exceptions;
using Picker.Application.DTOs.Comment;
using Picker.Application.DTOs.Food;
using Picker.Application.Services.Interfaces;
using Picker.Domain.Entities;
using Picker.Domain.Interfaces;

namespace Picker.Application.Services.Implementations;

public class FoodService : IFoodService
{
    private readonly IUnitOfWork _uow;

    public FoodService(IUnitOfWork uow) => _uow = uow;

    public async Task<IEnumerable<FoodDto>> GetAllAsync(Guid? cuisineId = null)
    {
        var foods = await _uow.Foods.GetAllWithDetailsAsync(cuisineId);
        return foods.Select(MapToDto);
    }

    public async Task<FoodDto> GetByIdAsync(Guid id)
    {
        var food = await _uow.Foods.GetByIdWithDetailsAsync(id)
            ?? throw new NotFoundException(nameof(Food), id);
        return MapToDto(food);
    }

    public async Task<FoodDto> GetRandomAsync(Guid? cuisineId = null)
    {
        var food = await _uow.Foods.GetRandomAsync(cuisineId)
            ?? throw new NotFoundException(nameof(Food), "random");
        return MapToDto(food);
    }

    public async Task<FoodDto> CreateAsync(CreateFoodDto dto)
    {
        _ = await _uow.Cuisines.GetByIdAsync(dto.CuisineId)
            ?? throw new NotFoundException(nameof(Cuisine), dto.CuisineId);

        var food = new Food
        {
            Title = dto.Title,
            Description = dto.Description,
            ImageUrl = dto.ImageUrl,
            CuisineId = dto.CuisineId
        };
        await _uow.Foods.AddAsync(food);
        await _uow.SaveChangesAsync();

        var created = await _uow.Foods.GetByIdWithDetailsAsync(food.Id);
        return MapToDto(created!);
    }

    public async Task<FoodDto> UpdateAsync(Guid id, UpdateFoodDto dto)
    {
        var food = await _uow.Foods.GetByIdWithDetailsAsync(id)
            ?? throw new NotFoundException(nameof(Food), id);

        _ = await _uow.Cuisines.GetByIdAsync(dto.CuisineId)
            ?? throw new NotFoundException(nameof(Cuisine), dto.CuisineId);

        food.Title = dto.Title;
        food.Description = dto.Description;
        food.ImageUrl = dto.ImageUrl;
        food.CuisineId = dto.CuisineId;
        food.UpdatedAt = DateTime.UtcNow;

        _uow.Foods.Update(food);
        await _uow.SaveChangesAsync();
        return MapToDto(food);
    }

    public async Task DeleteAsync(Guid id)
    {
        var food = await _uow.Foods.GetByIdAsync(id)
            ?? throw new NotFoundException(nameof(Food), id);
        _uow.Foods.Delete(food);
        await _uow.SaveChangesAsync();
    }

    private static FoodDto MapToDto(Food f) => new()
    {
        Id = f.Id,
        Title = f.Title,
        Description = f.Description,
        ImageUrl = f.ImageUrl,
        CuisineId = f.CuisineId,
        CuisineName = f.Cuisine?.Name ?? string.Empty,
        Comments = f.Comments.Select(c => new CommentDto
        {
            Id = c.Id,
            Content = c.Content,
            AuthorName = c.AuthorName,
            CategoryType = c.CategoryType,
            ItemId = c.ItemId,
            CreatedAt = c.CreatedAt,
            UpdatedAt = c.UpdatedAt
        }).ToList(),
        CreatedAt = f.CreatedAt,
        UpdatedAt = f.UpdatedAt
    };
}
