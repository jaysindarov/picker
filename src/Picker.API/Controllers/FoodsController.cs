using Microsoft.AspNetCore.Mvc;
using Picker.Application.DTOs.Food;
using Picker.Application.Services.Interfaces;

namespace Picker.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FoodsController : ControllerBase
{
    private readonly IFoodService _service;

    public FoodsController(IFoodService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] Guid? cuisineId = null)
    {
        return Ok(await _service.GetAllAsync(cuisineId));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        return Ok(await _service.GetByIdAsync(id));
    }

    [HttpGet("random")]
    public async Task<IActionResult> GetRandom([FromQuery] Guid? cuisineId = null)
    {
        return Ok(await _service.GetRandomAsync(cuisineId));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateFoodDto dto)
    {
        var result = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateFoodDto dto)
    {
        return Ok(await _service.UpdateAsync(id, dto));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _service.DeleteAsync(id);
        return NoContent();
    }
}
