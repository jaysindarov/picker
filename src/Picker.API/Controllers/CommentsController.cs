using Microsoft.AspNetCore.Mvc;
using Picker.Application.DTOs.Comment;
using Picker.Application.Services.Interfaces;
using Picker.Domain.Enums;

namespace Picker.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CommentsController : ControllerBase
{
    private readonly ICommentService _service;

    public CommentsController(ICommentService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetByItem([FromQuery] Guid itemId, [FromQuery] CategoryType categoryType) =>
        Ok(await _service.GetByItemAsync(itemId, categoryType));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCommentDto dto)
    {
        var result = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id) =>
        Ok(await _service.GetByIdAsync(id));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _service.DeleteAsync(id);
        return NoContent();
    }
}
