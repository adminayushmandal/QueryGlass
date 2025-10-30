
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using QueryGlass.Application.Common.Models;
using QueryGlass.Application.SystemInformation.Commands.AddNewServer;
using QueryGlass.Application.SystemInformation.Commands.DeleteServer;
using QueryGlass.Domain.Constants;

namespace QueryGlass.Web.Endpoints;

public class Windows : EndpointGroupBase
{
    public override void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder
            .MapPost(AddNewServer, nameof(AddNewServer).ToLower())
            .RequireAuthorization(Policies.AdminCanPurge)
            .WithSummary("Add new server")
            .WithDescription("Add new windows server of local or network.")
            .Produces<Result>(StatusCodes.Status201Created)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .WithOpenApi();

        groupBuilder
        .MapDelete(DeleteServer, nameof(DeleteServer).ToLower())
        .RequireAuthorization(Policies.AdminCanPurge)
        .WithSummary("Delete server")
        .WithDescription("Delete the existing server from the application, but it kept the historical data.")
        .Produces<Result>(StatusCodes.Status201Created)
        .Produces<Result>(StatusCodes.Status400BadRequest)
        .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
        .WithOpenApi();
    }

    async Task<Results<Created<Result>, BadRequest>> AddNewServer(ISender sender, AddNewServerCommand command)
    {
        var result = await sender.Send(command);
        return result.Succeeded
            ? TypedResults.Created(string.Empty, result)
            : TypedResults.BadRequest();
    }

    async Task<Results<Ok<Result>, BadRequest<Result>, BadRequest>> DeleteServer(ISender sender, Guid serverId)
    {
        var response = await sender.Send(new DeleteServerCommand(serverId));
        return response.Succeeded
        ? TypedResults.Ok(response)
        : TypedResults.BadRequest(response);
    }
}
