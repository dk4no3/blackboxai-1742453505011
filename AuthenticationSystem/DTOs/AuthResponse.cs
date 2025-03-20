namespace AuthenticationSystem.DTOs;

public class AuthResponse
{
    public bool Success { get; set; }
    public string? Token { get; set; }
    public string? Message { get; set; }
    public List<string>? Roles { get; set; }
}