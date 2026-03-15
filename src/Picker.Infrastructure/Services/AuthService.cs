using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Picker.Application.Common.Exceptions;
using Picker.Application.DTOs.Auth;
using Picker.Application.Services.Interfaces;
using Picker.Infrastructure.Identity;

namespace Picker.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IConfiguration _config;

    public AuthService(UserManager<AppUser> userManager, IConfiguration config)
    {
        _userManager = userManager;
        _config = config;
    }

    // ── Email / Password ────────────────────────────────────────────────────

    public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
    {
        if (await _userManager.FindByEmailAsync(dto.Email) is not null)
            throw new BadRequestException("An account with this email already exists.");

        var user = new AppUser
        {
            UserName = dto.Email,
            Email = dto.Email,
            DisplayName = dto.DisplayName,
            EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
            throw new BadRequestException(string.Join(" | ", result.Errors.Select(e => e.Description)));

        var role = IsAdminEmail(dto.Email) ? "Admin" : "User";
        await _userManager.AddToRoleAsync(user, role);

        return BuildResponse(user, role);
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email)
            ?? throw new BadRequestException("Invalid email or password.");

        if (!await _userManager.CheckPasswordAsync(user, dto.Password))
            throw new BadRequestException("Invalid email or password.");

        var roles = await _userManager.GetRolesAsync(user);
        var role = roles.FirstOrDefault() ?? "User";

        return BuildResponse(user, role);
    }

    // ── Google OAuth ─────────────────────────────────────────────────────────

    public async Task<AuthResponseDto> CreateOrUpdateGoogleUserAsync(string googleId, string email, string name)
    {
        var user = await _userManager.FindByLoginAsync("Google", googleId);

        if (user is null)
        {
            user = await _userManager.FindByEmailAsync(email);

            if (user is null)
            {
                user = new AppUser
                {
                    UserName = email,
                    Email = email,
                    DisplayName = name,
                    EmailConfirmed = true
                };
                var result = await _userManager.CreateAsync(user);
                if (!result.Succeeded)
                    throw new Exception($"Failed to create user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }

            await _userManager.AddLoginAsync(user, new UserLoginInfo("Google", googleId, "Google"));

            var existing = await _userManager.GetRolesAsync(user);
            if (!existing.Any())
            {
                var role = IsAdminEmail(email) ? "Admin" : "User";
                await _userManager.AddToRoleAsync(user, role);
            }
        }

        var roles = await _userManager.GetRolesAsync(user);
        var assignedRole = roles.FirstOrDefault() ?? "User";
        return BuildResponse(user, assignedRole);
    }

    // ── Helpers ──────────────────────────────────────────────────────────────

    /// <summary>
    /// Checks the AdminSettings:AdminEmails array in appsettings.
    /// Any email listed there is automatically given the Admin role at registration/first login.
    /// </summary>
    private bool IsAdminEmail(string email)
    {
        var adminEmails = _config.GetSection("AdminSettings:AdminEmails").Get<string[]>() ?? [];
        return adminEmails.Contains(email, StringComparer.OrdinalIgnoreCase);
    }

    private AuthResponseDto BuildResponse(AppUser user, string role)
    {
        var (token, expiresAt) = GenerateJwtToken(user, role);
        return new AuthResponseDto
        {
            Token = token,
            ExpiresAt = expiresAt,
            UserId = user.Id,
            Email = user.Email!,
            DisplayName = user.DisplayName,
            Role = role
        };
    }

    private (string token, DateTime expiresAt) GenerateJwtToken(AppUser user, string role)
    {
        var jwtSettings = _config.GetSection("JwtSettings");
        var secret = jwtSettings["Secret"] ?? throw new InvalidOperationException("JWT Secret not configured.");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var expiryMinutes = int.TryParse(jwtSettings["ExpiryMinutes"], out var m) ? m : 60;
        var expiresAt = DateTime.UtcNow.AddMinutes(expiryMinutes);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Email, user.Email!),
            new Claim(JwtRegisteredClaimNames.Name, user.DisplayName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Role, role)
        };

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: expiresAt,
            signingCredentials: creds
        );

        return (new JwtSecurityTokenHandler().WriteToken(token), expiresAt);
    }
}
