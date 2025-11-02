using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using QueryGlass.Application.Common.Interfaces;
using QueryGlass.Application.Common.Models;
using QueryGlass.Application.Configuration.Commands.ConfigureUser;
using QueryGlass.Application.Configuration.Commands.UpdateUser;
using QueryGlass.Domain.Constants;
using QueryGlass.Domain.Entities;

namespace QueryGlass.Infrastructure.Identity;

public class IdentityService(
    UserManager<ApplicationUser> userManager,
    IUserClaimsPrincipalFactory<ApplicationUser> userClaimsPrincipalFactory,
    IAuthorizationService authorizationService, IMapper mapper) : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly IUserClaimsPrincipalFactory<ApplicationUser> _userClaimsPrincipalFactory = userClaimsPrincipalFactory;
    private readonly IAuthorizationService _authorizationService = authorizationService;
    private readonly IMapper _mapper = mapper;

    public async Task<string?> GetUserNameAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);

        return user?.UserName;
    }

    public async Task<(Result Result, string UserId)> CreateUserAsync(ConfigureUserCommand request, CancellationToken cancellationToken = default)
    {
        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            DisplayName = request.DisplayName,
            PhoneNumber = request.PhoneNumber
        };

        var result = await _userManager.CreateAsync(user, request.Password);

        return (result.ToApplicationResult(), user.Id.ToString());
    }

    public async Task<bool> IsInRoleAsync(string userId, string role)
    {
        var user = await _userManager.FindByIdAsync(userId);

        return user != null && await _userManager.IsInRoleAsync(user, role);
    }

    public async Task<bool> AuthorizeAsync(string userId, string policyName)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
        {
            return false;
        }

        var principal = await _userClaimsPrincipalFactory.CreateAsync(user);

        var result = await _authorizationService.AuthorizeAsync(principal, policyName);

        return result.Succeeded;
    }

    public async Task<Result> DeleteUserAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);

        return user != null ? await DeleteUserAsync(user) : Result.Success();
    }

    public async Task<Result> DeleteUserAsync(ApplicationUser user)
    {
        var result = await _userManager.DeleteAsync(user);

        return result.ToApplicationResult();
    }

    public async Task<UserDto?> GetUserAsync(string userId, CancellationToken cancellationToken = default)
    {
        var appUser = await _userManager.FindByIdAsync(userId)
        ?? throw new KeyNotFoundException($"User not found with this id '{userId}'");

        var user = _mapper.Map<UserDto>(appUser);
        var roles = await _userManager.GetRolesAsync(appUser);
        user.Roles = [.. roles];
        return user;
    }

    public async Task<IQueryable<ApplicationUser>> GetUsersAsync(string currentUser, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(currentUser)) throw new UnauthorizedAccessException(Error.UnauthorizeError);

        if (!Guid.TryParse(currentUser, out var userId)) throw new UnauthorizedAccessException(Error.UnauthorizeError);

        var users = _userManager.Users
                .Where(x => x.Id != userId)
                .AsNoTracking();

        return await Task.Run(() => users);
    }

    public async Task<bool> AddToRoleAsync(string userId, string role)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return false;
        var result = await _userManager.AddToRoleAsync(user, role);
        return result.Succeeded;
    }

    public async Task<List<string>> GetRolesAsync(string userId, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user == null) return [];

        var roles = await _userManager.GetRolesAsync(user);

        return [.. roles];
    }

    public async Task<(Result, string)> UpdateUserAsync(UpdateUserCommand request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(request.Email))
        {
            return (Result.Failure(["Email cannot be empty."]), string.Empty);
        }

        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user == null)
        {
            return (Result.Failure(["User not found."]), string.Empty);
        }
        user.DisplayName = request.DisplayName;
        user.PhoneNumber = request.PhoneNumber;
        var result = await _userManager.UpdateAsync(user);
        return (result.ToApplicationResult(), user.Id.ToString());
    }

    public async Task<bool> UpdateRoleAsync(string userId, string newRole)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return false;

        var currentRoles = await _userManager.GetRolesAsync(user);
        var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
        if (!removeResult.Succeeded) return false;

        var addResult = await _userManager.AddToRoleAsync(user, newRole);
        return addResult.Succeeded;
    }
}
