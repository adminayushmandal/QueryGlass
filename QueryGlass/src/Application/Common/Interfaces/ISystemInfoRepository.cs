using QueryGlass.Domain.Entities;

namespace QueryGlass.Application.Common.Interfaces;

public interface IWindowsRepository
{
    Task<SystemInfo?> GetSystemInfoByIdAsync(Guid systemId, CancellationToken cancellationToken = default);

    Task<SystemInfo?> GetSystemInfoByNameAsync(string serverName, CancellationToken cancellationToken = default);

    Task<IEnumerable<SystemInfo>> GetSystemsAsync(CancellationToken cancellationToken = default);

    Task<SystemInfo?> CreateAsync(SystemInfo systemInfo, CancellationToken cancellationToken = default);

    Task<SystemInfo> UpdateAsync(SystemInfo systemInfo, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(Guid systemId, CancellationToken cancellationToken = default);
    Task<bool> IsExistAsync(string machineName, CancellationToken cancellationToken = default);
}
