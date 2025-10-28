namespace QueryGlass.Domain.Constants;

public record Error
{
    public const string UnauthorizeError = "Session is expired or user is not logged in.";
    public static string NotFoundError(string message) => $"Not found : {message}";
    public static string BadRequest(string message) => $"Bad request : {message}";
}
