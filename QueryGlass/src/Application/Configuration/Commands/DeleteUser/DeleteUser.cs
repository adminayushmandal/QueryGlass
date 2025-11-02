using QueryGlass.Application.Common.Interfaces;

namespace QueryGlass.Application.Configuration.Commands.DeleteUser;

public record DeleteUserCommand(Guid UserId) : IRequest<Result>;

internal sealed class DeleteUserCommandHandler(IIdentityService identityService) : IRequestHandler<DeleteUserCommand, Result>
{
    private readonly IIdentityService _identityService = identityService;
    public async Task<Result> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        return await _identityService.DeleteUserAsync(request.UserId.ToString());
    }
}
