using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using QueryGlass.Application.Common.Models;
using QueryGlass.Application.Configuration.Commands.ConfigureUser;
using QueryGlass.Application.Configuration.Commands.DeleteUser;
using QueryGlass.Application.Configuration.Commands.UpdateUser;
using QueryGlass.Application.Configuration.Queries.GetUsers;
using QueryGlass.Domain.Constants;

namespace QueryGlass.Web.Endpoints;

public class Configuration : EndpointGroupBase
{
    public override void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapGet(GetUsers, nameof(GetUsers).ToLower())
        .RequireAuthorization()
        .WithSummary("Get users")
        .WithDescription("Administrator can get the list of users.")
        .Produces<ConfigurationLookupDto>()
        .Produces<ProblemDetails>(StatusCodes.Status404NotFound);

        groupBuilder.MapPost(CreateNewUser, nameof(CreateNewUser).ToLower())
        .RequireAuthorization(Policies.AdminCanPurge)
        .WithSummary("Create new user")
        .WithDescription("Administrator can create new user with role assignment.")
        .Produces<Result>(StatusCodes.Status201Created)
        .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
        .WithOpenApi();

        groupBuilder.MapDelete(DeleteUser, nameof(DeleteUser).ToLower())
        .RequireAuthorization(Policies.AdminCanPurge)
        .WithSummary("Delete user")
        .WithDescription("Administrator can delete a user by user ID.")
        .Produces<Result>(StatusCodes.Status200OK)
        .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
        .WithOpenApi();

        groupBuilder.MapPut(UpdateUser, nameof(UpdateUser).ToLower())
        .RequireAuthorization(Policies.AdminCanPurge)
        .WithSummary("Update user")
        .WithDescription("Administrator can update user details and role assignment.")
        .Produces<Result>(StatusCodes.Status200OK)
        .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
        .WithOpenApi();
    }

    async Task<Results<Ok<ConfigurationLookupDto>, BadRequest, NotFound>> GetUsers(ISender sender)
    {
        var response = await sender.Send(new GetUsersQuery());
        return response is null
        ? TypedResults.NotFound()
        : TypedResults.Ok(response);
    }

    async Task<Results<Created<Result>, BadRequest>> CreateNewUser(ISender sender, ConfigureUserCommand command)
    {
        var result = await sender.Send(command);
        return result.Succeeded
        ? TypedResults.Created(string.Empty, result)
        : TypedResults.BadRequest();
    }

    async Task<Results<Ok<Result>, BadRequest>> DeleteUser(ISender sender, Guid userId)
    {
        var result = await sender.Send(new DeleteUserCommand(userId));
        return result.Succeeded
        ? TypedResults.Ok(result)
        : TypedResults.BadRequest();
    }

    async Task<Results<Ok<Result>, BadRequest>> UpdateUser(ISender sender, UpdateUserCommand command)
    {
        var result = await sender.Send(command);
        return result.Succeeded
        ? TypedResults.Ok(result)
        : TypedResults.BadRequest();
    }
}
