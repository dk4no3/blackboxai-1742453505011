using AuthenticationSystem.DTOs;

namespace AuthenticationSystem.Services.Interfaces;

public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request);
    Task<AuthResponse> LoginAsync(LoginRequest request);
    Task<bool> ValidateTokenAsync(string token);
    string GenerateJwtToken(string username, IEnumerable<string> roles);
}