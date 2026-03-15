using Microsoft.AspNetCore.Mvc;
using Picker.Application.DTOs.Book;
using Picker.Application.Services.Interfaces;

namespace Picker.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BooksController : ControllerBase
{
    private readonly IBookService _service;

    public BooksController(IBookService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] Guid? genreId = null) =>
        Ok(await _service.GetAllAsync(genreId));

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id) =>
        Ok(await _service.GetByIdAsync(id));

    [HttpGet("random")]
    public async Task<IActionResult> GetRandom([FromQuery] Guid? genreId = null) =>
        Ok(await _service.GetRandomAsync(genreId));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateBookDto dto)
    {
        var result = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateBookDto dto) =>
        Ok(await _service.UpdateAsync(id, dto));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _service.DeleteAsync(id);
        return NoContent();
    }
}
