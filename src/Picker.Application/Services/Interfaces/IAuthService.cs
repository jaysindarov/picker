using Picker.Application.DTOs.Auth;

namespace Picker.Application.Services.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto> CreateOrUpdateGoogleUserAsync(string googleId, string email, string name);
}
