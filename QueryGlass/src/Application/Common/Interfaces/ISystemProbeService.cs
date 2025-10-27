namespace QueryGlass.Application.Common.Interfaces;

public interface ISystemProbeService
{
    Task<bool> CheckServerAvailabilityAsync(string machineName, CancellationToken cancellationToken = default);
    Task<string> GetOsVersionAsync(string machineName, CancellationToken cancellationToken = default);
}
