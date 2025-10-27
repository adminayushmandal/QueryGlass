
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using QueryGlass.Application.Common.Models;
using QueryGlass.Application.SystemInformation.Commands.AddNewServer;

namespace QueryGlass.Web.Endpoints;

public class Server : EndpointGroupBase
{
    public override void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder
            .MapPost(AddNewServer, nameof(AddNewServer).ToLower())
            .WithSummary("Add new server")
            .WithDescription("Add new windows server of local or network.")
            .Produces<Result>(StatusCodes.Status201Created)
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
}
