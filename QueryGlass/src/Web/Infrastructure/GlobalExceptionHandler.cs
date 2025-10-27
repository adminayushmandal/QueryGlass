using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http.Features;

namespace QueryGlass.Web.Infrastructure;

internal sealed class GlobalExceptionHandler(IProblemDetailsService service, ILogger<GlobalExceptionHandler> logger, IWebHostEnvironment environment) : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger = logger;
    private readonly IProblemDetailsService _service = service;
    private readonly IWebHostEnvironment _environment = environment;
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var (statuCode, title) = exception switch
        {
            ApplicationException or InvalidOperationException => (StatusCodes.Status400BadRequest, "Bad Request"),
            KeyNotFoundException => (StatusCodes.Status404NotFound, "not found"),
            _ => (StatusCodes.Status500InternalServerError, "Internal server error")
        };

        var detailedMessage = _environment.IsDevelopment()
            ? exception.ToString()
            : exception.Message;

        var activity = httpContext.Features.Get<IHttpActivityFeature>()?.Activity;

        httpContext.Response.StatusCode = statuCode;

        _logger.LogError("An unhandled exception occured.\nError '{error}'", exception);

        return await _service.TryWriteAsync(new()
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = new()
            {
                Title = title,
                Detail = detailedMessage,
                Status = statuCode,
                Extensions =
                {
                    ["request_id"]= activity?.Id
                }
            }
        });
    }
}
