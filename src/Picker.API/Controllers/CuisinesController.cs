using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Picker.Application.DTOs.Cuisine;
using Picker.Application.Services.Interfaces;

namespace Picker.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CuisinesController : ControllerBase
{
    private readonly ICuisineService _service;

    public CuisinesController(ICuisineService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _service.GetAllAsync());
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        return Ok(await _service.GetByIdAsync(id));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateCuisineDto dto)
    {
        var result = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCuisineDto dto)
    {
        return Ok(await _service.UpdateAsync(id, dto));
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _service.DeleteAsync(id);
        return NoContent();
    }
}
