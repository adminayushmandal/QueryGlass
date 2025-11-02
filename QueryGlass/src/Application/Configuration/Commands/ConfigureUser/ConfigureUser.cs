using Microsoft.VisualBasic;
using QueryGlass.Application.Common.Interfaces;

namespace QueryGlass.Application.Configuration.Commands.ConfigureUser;

public record ConfigureUserCommand : UserConfiguration, IRequest<Result>;

internal sealed class ConfigureUserCommandHandler(IIdentityService identityService) : IRequestHandler<ConfigureUserCommand, Result>
{
    private readonly IIdentityService _identityService = identityService;
    public async Task<Result> Handle(ConfigureUserCommand request, CancellationToken cancellationToken)
    {
        var (result, userId) = await _identityService.CreateUserAsync(request, cancellationToken);
        if (result.Succeeded)
        {
            await _identityService.AddToRoleAsync(userId, request.Role ?? string.Empty);
        }
        return result;
    }
}
