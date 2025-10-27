using System.Management;
using System.Net.NetworkInformation;
using System.Runtime.Versioning;
using Microsoft.Extensions.Logging;
using QueryGlass.Application.Common.Interfaces;

namespace QueryGlass.Infrastructure.Serivces;

[SupportedOSPlatform("windows")]
internal sealed class SystemProbeService(ILogger<SystemProbeService> logger) : ISystemProbeService
{
    private readonly ILogger<SystemProbeService> _logger = logger;

    public async Task<bool> CheckServerAvailabilityAsync(string machineName,
        CancellationToken cancellationToken = default)
    {
        try
        {
            using var ping = new Ping();
            var result = await ping.SendPingAsync(machineName, 1000);
            return result.Status == IPStatus.Success;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "No server with this name '{machineName}' founded", machineName);
            return false;
        }
    }

    public Task<string> GetOsVersionAsync(string machineName, CancellationToken cancellationToken = default)
    {
        try
        {
            using var searcher = new ManagementObjectSearcher(
                "SELECT Caption FROM Win32_OperatingSystem"
            );

            foreach (ManagementObject os in searcher.Get())
            {
                return Task.FromResult(os["Caption"]?.ToString() ?? "Unknown");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get the OS version of '{machineName}'", machineName);
        }

        return Task.FromResult(string.Empty);
    }
}
