using QueryGlass.Application.Common.Interfaces;

namespace QueryGlass.Application.Windows.Commands.AddNewServer;

public record AddNewServerCommand : WindowsDto, IRequest<Result>;

public class AddNewServerCommandHandler(IWindowsRepository repository,
ISystemProbeService probeService, IMapper mapper)
    : IRequestHandler<AddNewServerCommand, Result>
{
    private readonly IWindowsRepository _repository = repository;
    private readonly ISystemProbeService _probeService = probeService;
    private readonly IMapper _mapper = mapper;

    public async Task<Result> Handle(AddNewServerCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.ServerName))
        {
            throw new ArgumentNullException("Server name cannot be null or empty.", nameof(request.ServerName));
        }

        var exists = await _repository.IsExistAsync(request.ServerName, cancellationToken);

        if (exists)
        {
            throw new InvalidOperationException($"Server '{request.ServerName}' already exists.");
        }

        var isAvailable = await _probeService.CheckServerAvailabilityAsync(request.ServerName, cancellationToken);
        if (!isAvailable)
        {
            throw new InvalidOperationException($"Server '{request.ServerName}' is not reachable.");
        }
        string operatingSystem = await _probeService.GetLocalMachineOsVersionAsync(request.ServerName, cancellationToken);

        var server = new Domain.Entities.WindowsServer
        {
            MachineName = request.ServerName,
            OSVersion = operatingSystem,
        };

        server = _mapper.Map(request, server);

        var entity = await _repository.CreateAsync(server, cancellationToken);
        if (entity is null) throw new ApplicationException("Failed to add the server.");
        return Result.Success();
    }
}
