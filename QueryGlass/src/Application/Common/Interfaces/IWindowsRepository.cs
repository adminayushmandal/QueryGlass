using QueryGlass.Domain.Entities;

namespace QueryGlass.Application.Common.Interfaces;

public interface IWindowsRepository
{
    Task<WindowsServer?> GetSystemInfoByIdAsync(Guid systemId, CancellationToken cancellationToken = default);

    Task<WindowsServer?> GetSystemInfoByNameAsync(string serverName, CancellationToken cancellationToken = default);

    Task<IEnumerable<WindowsServer>> GetSystemsAsync(CancellationToken cancellationToken = default);

    Task<WindowsServer?> CreateAsync(WindowsServer systemInfo, CancellationToken cancellationToken = default);

    Task<WindowsServer> UpdateAsync(WindowsServer systemInfo, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(Guid systemId, CancellationToken cancellationToken = default);

    Task<bool> IsExistAsync(string machineName, CancellationToken cancellationToken = default);

    Task<IQueryable<WindowsServer>> GetWindowsServersAsync(CancellationToken cancellationToken = default);
}
