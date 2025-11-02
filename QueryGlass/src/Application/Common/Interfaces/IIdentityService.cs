using QueryGlass.Application.Configuration.Commands.ConfigureUser;
using QueryGlass.Application.Configuration.Commands.UpdateUser;
using QueryGlass.Domain.Entities;

namespace QueryGlass.Application.Common.Interfaces;

public interface IIdentityService
{
    Task<string?> GetUserNameAsync(string userId);

    Task<bool> IsInRoleAsync(string userId, string role);

    Task<bool> AuthorizeAsync(string userId, string policyName);

    Task<(Result Result, string UserId)> CreateUserAsync(ConfigureUserCommand request, CancellationToken cancellationToken = default);

    Task<Result> DeleteUserAsync(string userId);

    Task<UserDto?> GetUserAsync(string userId, CancellationToken cancellationToken = default);

    Task<IQueryable<ApplicationUser>> GetUsersAsync(string userId, CancellationToken cancellationToken = default);

    Task<bool> AddToRoleAsync(string userId, string role);

    Task<List<string>> GetRolesAsync(string userId, CancellationToken cancellationToken = default);

    Task<(Result Result, string UserId)> UpdateUserAsync(UpdateUserCommand request, CancellationToken cancellationToken = default);

    Task<bool> UpdateRoleAsync(string userId, string newRole);
}
