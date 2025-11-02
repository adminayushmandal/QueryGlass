namespace QueryGlass.Application.Common.Models;

public record class UserConfiguration
{
    public string? DisplayName { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Role { get; set; }
    public string Password { get; set; } = string.Empty;
}
