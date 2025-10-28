using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using QueryGlass.Application.Common.Models;
using QueryGlass.Application.User.Queries.UserProfile;
using QueryGlass.Domain.Entities;

namespace QueryGlass.Web.Endpoints;

public class User : EndpointGroupBase
{
    public override void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapIdentityApi<ApplicationUser>();

        groupBuilder
        .MapGet(Me, nameof(Me).ToLower())
        .RequireAuthorization()
        .WithSummary("User profile")
        .WithDescription("Get the user profile after user logged in.")
        .Produces<UserDto>(StatusCodes.Status200OK)
        .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
        .Produces<ProblemDetails>(StatusCodes.Status401Unauthorized);
    }

    async Task<Results<Ok<UserDto>, NotFound, UnauthorizedHttpResult>> Me(ISender sender)
    {
        var user = await sender.Send(new UserProfileQuery());
        return user is null
        ? TypedResults.NotFound()
        : TypedResults.Ok(user);
    }
}
