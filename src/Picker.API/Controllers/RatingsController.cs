using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Picker.Application.DTOs.Rating;
using Picker.Application.Services.Interfaces;
using Picker.Domain.Enums;

namespace Picker.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RatingsController : ControllerBase
{
    private readonly IRatingService _service;
    private readonly ICurrentUserService _currentUser;

    public RatingsController(IRatingService service, ICurrentUserService currentUser)
    {
        _service = service;
        _currentUser = currentUser;
    }

    /// <summary>Get the current user's rating for a specific item.</summary>
    [HttpGet]
    public async Task<IActionResult> GetMyRating(
        [FromQuery] Guid itemId,
        [FromQuery] CategoryType categoryType)
    {
        var rating = await _service.GetUserRatingAsync(itemId, categoryType, _currentUser.UserId!);
        if (rating is null) return NotFound(new { message = "You have not rated this item yet." });
        return Ok(rating);
    }

    /// <summary>Create or update a rating (1–5) for a Food, Movie, or Book.</summary>
    [HttpPost]
    public async Task<IActionResult> Upsert([FromBody] CreateRatingDto dto)
    {
        var result = await _service.UpsertAsync(dto, _currentUser.UserId!);
        return Ok(result);
    }
}
