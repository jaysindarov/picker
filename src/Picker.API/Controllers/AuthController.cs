using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Picker.Application.DTOs.Auth;
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

    // ── Email / Password ─────────────────────────────────────────────────────

    /// <summary>
    /// Register a new account with email and password.
    /// The role is automatically set to "Admin" if the email is listed in
    /// AdminSettings:AdminEmails in appsettings.json; otherwise "User".
    /// </summary>
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        var result = await _authService.RegisterAsync(dto);
        return CreatedAtAction(nameof(CurrentUser), result);
    }

    /// <summary>Login with email and password. Returns a JWT token.</summary>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var result = await _authService.LoginAsync(dto);
        return Ok(result);
    }

    // ── Google OAuth ──────────────────────────────────────────────────────────

    /// <summary>
    /// Redirect to Google sign-in. After successful auth Google redirects back
    /// to /api/auth/callback/google which returns a JWT token.
    /// </summary>
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
        var firstName = info.Principal.FindFirstValue(ClaimTypes.GivenName) ?? string.Empty;
        var lastName = info.Principal.FindFirstValue(ClaimTypes.Surname) ?? string.Empty;
        var googleId = info.ProviderKey;

        if (string.IsNullOrEmpty(email))
            return BadRequest(new { error = "Google did not provide an email address." });

        var response = await _authService.CreateOrUpdateGoogleUserAsync(googleId, email, firstName, lastName);
        return Ok(response);
    }

    // ── Misc ──────────────────────────────────────────────────────────────────

    /// <summary>Returns the current authenticated user's profile.</summary>
    [HttpGet("me")]
    [Authorize]
    public IActionResult CurrentUser()
    {
        return Ok(new
        {
            UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
            Email = User.FindFirstValue(ClaimTypes.Email),
            Username = User.FindFirstValue("username"),
            FirstName = User.FindFirstValue(ClaimTypes.GivenName),
            LastName = User.FindFirstValue(ClaimTypes.Surname),
            Role = User.FindFirstValue(ClaimTypes.Role)
        });
    }
}
