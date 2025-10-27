using QueryGlass.Application.Common.Interfaces;

namespace QueryGlass.Application.SystemInformation.Commands.AddNewServer;

public record AddNewServerCommand : IRequest<Result>
{
    public string? ServerName { get; init; }
}

public class AddNewServerCommandValidator : AbstractValidator<AddNewServerCommand>
{
    public AddNewServerCommandValidator()
    {
        RuleFor(x => x.ServerName)
            .NotNull()
            .WithMessage("Server name should not be null");

        RuleFor(x => x.ServerName)
            .Matches(@"^[a-zA-Z0-9\-.]+$")
            .WithMessage("Invalid machine name.");
    }
}

public class AddNewServerCommandHandler(ISystemInfoRepository systemInfoRepository, ISystemProbeService systemProbe)
    : IRequestHandler<AddNewServerCommand, Result>
{
    private readonly ISystemInfoRepository _systemInfoRepository = systemInfoRepository;
    private readonly ISystemProbeService _systemProbe = systemProbe;

    public async Task<Result> Handle(AddNewServerCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.ServerName))
        {
            throw new InvalidOperationException("Server name is null or empty.");
        }

        var isCheck = await _systemProbe.CheckServerAvailabilityAsync(request.ServerName, cancellationToken);

        if (!isCheck)
        {
            throw new KeyNotFoundException($"Server with name '{request.ServerName}' not available.");
        }

        var os = await _systemProbe.GetOsVersionAsync(request.ServerName, cancellationToken);

        if (string.IsNullOrEmpty(os)) throw new ApplicationException("Failed to get OS version.");

        var entity = _systemInfoRepository.CreateAsync(new() { OSVersion = os, MachineName = request.ServerName, },
            cancellationToken);

        return entity is null ? throw new ApplicationException("Failed to add server.") : Result.Success();
    }
}
