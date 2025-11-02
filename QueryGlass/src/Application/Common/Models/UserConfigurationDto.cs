namespace QueryGlass.Application.Common.Models;

public record ConfigurationLookupDto
{
    public IReadOnlyCollection<string> ApplicationRoles { get; set; } = [];
    public IReadOnlyCollection<string> ApplicationPolicies { get; set; } = [];
    public IReadOnlyCollection<UserDto> Users { get; set; } = [];
}

