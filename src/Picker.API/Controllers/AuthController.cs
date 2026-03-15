using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Picker.Application.Services.Interfaces;
using Picker.Infrastructure.Identity;

namespace Picker.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly SignInManager<AppUser> _signInManager;

    public AuthController(IAuthService authService, SignInManager<AppUser> signInManager)
    {
        _authService = authService;
        _signInManager = signInManager;
    }

    /// <summary>Redirect to Google OAuth sign-in page.</summary>
    [HttpGet("signin/google")]
    [AllowAnonymous]
    public IActionResult GoogleSignIn()
    {
        var redirectUrl = Url.Action(nameof(GoogleCallback), "Auth", null, Request.Scheme);
        var properties = _signInManager.ConfigureExternalAuthenticationProperties(
            GoogleDefaults.AuthenticationScheme, redirectUrl);
        return Challenge(properties, GoogleDefaults.AuthenticationScheme);
    }

    /// <summary>Google OAuth callback — issues a JWT token.</summary>
    [HttpGet("callback/google")]
    [AllowAnonymous]
    public async Task<IActionResult> GoogleCallback()
    {
        var info = await _signInManager.GetExternalLoginInfoAsync();
        if (info is null)
            return BadRequest(new { error = "Google authentication failed. Could not retrieve login info." });

        var email = info.Principal.FindFirstValue(ClaimTypes.Email);
        var name = info.Principal.FindFirstValue(ClaimTypes.Name) ?? email ?? "Unknown";
        var googleId = info.ProviderKey;

        if (string.IsNullOrEmpty(email))
            return BadRequest(new { error = "Google did not provide an email address." });

        var response = await _authService.CreateOrUpdateGoogleUserAsync(googleId, email, name);
        return Ok(response);
    }

    /// <summary>Returns current authenticated user info.</summary>
    [HttpGet("me")]
    [Authorize]
    public IActionResult Me()
    {
        return Ok(new
        {
            UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
            Email = User.FindFirstValue(ClaimTypes.Email),
            Name = User.FindFirstValue(ClaimTypes.Name),
            Role = User.FindFirstValue(ClaimTypes.Role)
        });
    }
}
