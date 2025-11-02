using System.Reflection;
using QueryGlass.Application.Common.Interfaces;
using QueryGlass.Application.Common.Mappings;
using QueryGlass.Domain.Constants;

namespace QueryGlass.Application.Configuration.Queries.GetUsers;

public record GetUsersQuery : IRequest<ConfigurationLookupDto>;

internal sealed class GetUsersQueryValidator : AbstractValidator<GetUsersQuery>
{
    public GetUsersQueryValidator()
    {
    }
}

internal sealed class GetUsersQueryHandler(IIdentityService identityService, IMapper mapper, IUser user) : IRequestHandler<GetUsersQuery, ConfigurationLookupDto>
{
    private readonly IIdentityService _identityService = identityService;
    private readonly IMapper _mapper = mapper;
    private readonly IUser _currentUser = user;
    public async Task<ConfigurationLookupDto> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        var policies = typeof(Policies)
        .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
        .Where(fi => fi.IsLiteral && !fi.IsInitOnly && fi.FieldType == typeof(string))
        .Select(s => s.Name).ToList().AsReadOnly();

        var roles = typeof(Roles)
        .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
        .Where(fi => fi.IsLiteral && !fi.IsInitOnly && fi.FieldType == typeof(string))
        .Select(s => s.Name).ToList().AsReadOnly();

        var users = await _identityService.GetUsersAsync(_currentUser.Id ?? string.Empty, cancellationToken);

        var appUsers = await users
        .OrderByDescending(x => x.Created)
        .ProjectToListAsync<UserDto>(_mapper.ConfigurationProvider, cancellationToken);

        foreach (var user in appUsers)
        {
            var userRoles = await _identityService.GetRolesAsync(user.Id.ToString(), cancellationToken);
            user.Roles = userRoles;
        }

        return await Task.Run(() => new ConfigurationLookupDto()
        {
            ApplicationPolicies = policies,
            ApplicationRoles = roles,
            Users = appUsers
        });
    }
}
