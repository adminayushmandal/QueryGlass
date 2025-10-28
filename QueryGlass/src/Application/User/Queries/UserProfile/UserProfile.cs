using QueryGlass.Application.Common.Interfaces;
using QueryGlass.Domain.Constants;

namespace QueryGlass.Application.User.Queries.UserProfile;

public record UserProfileQuery : IRequest<UserDto?>;


internal sealed class UserProfileQueryHandler(IIdentityService identity, IUser currentUser) : IRequestHandler<UserProfileQuery, UserDto?>
{
    public async Task<UserDto?> Handle(UserProfileQuery request, CancellationToken cancellationToken)
    => string.IsNullOrEmpty(currentUser.Id)
            ? throw new UnauthorizedAccessException(Error.UnauthorizeError)
            : await identity.GetUserAsync(currentUser.Id, cancellationToken);
}
