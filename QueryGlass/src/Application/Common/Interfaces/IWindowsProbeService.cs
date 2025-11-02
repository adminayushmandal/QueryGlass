using QueryGlass.Domain.Entities;

namespace QueryGlass.Application.Common.Interfaces;

public interface ISystemProbeService
{
    Task<bool> CheckServerAvailabilityAsync(string machineName, CancellationToken cancellationToken = default);
    Task<string> GetLocalMachineOsVersionAsync(string machineName, CancellationToken cancellationToken = default);
    Task<string> GetOperatingSystemRemoteAsync(string hostName, string username, string password, CancellationToken cancellationToken = default);
    Task<SystemMetric> CollectSystemMetricsAsync(string hostName, CancellationToken cancellationToken = default);
}
