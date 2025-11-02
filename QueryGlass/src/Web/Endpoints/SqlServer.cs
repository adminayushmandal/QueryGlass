
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using QueryGlass.Application.Common.Models;
using QueryGlass.Application.SqlServer.Commands.AddNewSqlInstance;

namespace QueryGlass.Web.Endpoints;

public class SqlServer : EndpointGroupBase
{
    public override void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder
        .MapPost(AddNewSqlInstance, nameof(AddNewSqlInstance).ToLower())
        .RequireAuthorization()
        .WithSummary("Add new sql instance")
        .WithDescription("Add new sql instance of current system.")
        .Produces<Result>()
        .Produces<Result>(StatusCodes.Status400BadRequest)
        .WithOpenApi();
    }

    public async Task<Results<Created<Result>, BadRequest<Result>>> AddNewSqlInstance(ISender sender, AddNewSqlInstanceCommand command)
    {
        var response = await sender.Send(command);
        return response.Succeeded
        ? TypedResults.Created(string.Empty, response)
        : TypedResults.BadRequest(response);
    }
}
