using QueryGlass.Application.Common.Interfaces;
using QueryGlass.Application.Common.Mappings;

namespace QueryGlass.Application.Windows.Queries.GetWindowsServers;

public record GetWindowsServersQuery : IRequest<WindowsLookupDto>;

internal sealed class GetWindowsServersQueryHandler(IWindowsRepository repository, IMapper mapper) : IRequestHandler<GetWindowsServersQuery, WindowsLookupDto>
{
    private readonly IWindowsRepository _windows = repository;
    private readonly IMapper _mapper = mapper;
    public async Task<WindowsLookupDto> Handle(GetWindowsServersQuery request, CancellationToken cancellationToken)
    {
        var servers = await _windows.GetWindowsServersAsync(cancellationToken);
        var windows = _mapper.Map<IReadOnlyCollection<WindowsDto>>(servers.OrderByDescending(x => x.Created));
        return new()
        {
            Windows = windows
        };
    }
}
