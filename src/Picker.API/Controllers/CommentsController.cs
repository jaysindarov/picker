using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Picker.Application.DTOs.Comment;
using Picker.Application.Services.Interfaces;
using Picker.Domain.Enums;

namespace Picker.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CommentsController : ControllerBase
{
    private readonly ICommentService _service;
    private readonly ICurrentUserService _currentUser;

    public CommentsController(ICommentService service, ICurrentUserService currentUser)
    {
        _service = service;
        _currentUser = currentUser;
    }

    [HttpGet]
    public async Task<IActionResult> GetByItem([FromQuery] Guid itemId, [FromQuery] CategoryType categoryType)
    {
        return Ok(await _service.GetByItemAsync(itemId, categoryType));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        return Ok(await _service.GetByIdAsync(id));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCommentDto dto)
    {
        var result = await _service.CreateAsync(dto, _currentUser.UserId!);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCommentDto dto)
    {
        var result = await _service.UpdateAsync(id, dto, _currentUser.UserId!, _currentUser.IsAdmin);
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _service.DeleteAsync(id, _currentUser.UserId!, _currentUser.IsAdmin);
        return NoContent();
    }
}
