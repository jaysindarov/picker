using Picker.Application.DTOs.Auth;

namespace Picker.Application.Services.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterDto dto);
    Task<AuthResponseDto> LoginAsync(LoginDto dto);
    Task<AuthResponseDto> CreateOrUpdateGoogleUserAsync(string googleId, string email, string firstName, string lastName);
}
