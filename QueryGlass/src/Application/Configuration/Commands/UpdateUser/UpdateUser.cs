using QueryGlass.Application.Common.Interfaces;

namespace QueryGlass.Application.Configuration.Commands.UpdateUser;

public record UpdateUserCommand : UserConfiguration, IRequest<Result>;

public class UpdateUserCommandHandler(IIdentityService identityService) : IRequestHandler<UpdateUserCommand, Result>
{
    private readonly IIdentityService _identityService = identityService;
    public async Task<Result> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var (result, userId) = await _identityService.UpdateUserAsync(request, cancellationToken);
        if (result.Succeeded && !string.IsNullOrEmpty(request.Role))
        {
            await _identityService.UpdateRoleAsync(userId, request.Role);
        }
        return result;
    }
}
