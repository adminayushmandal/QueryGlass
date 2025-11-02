using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using QueryGlass.Application.Common.Interfaces;

namespace QueryGlass.Infrastructure.Serivces;

internal sealed class WindowsMetricWorker(
    ILogger<WindowsMetricWorker> logger,
    IServiceScopeFactory scopeFactory)
    : BackgroundService, IWindowsMetricWorker
{
    private readonly ILogger<WindowsMetricWorker> _logger = logger;
    private readonly IServiceScopeFactory _scopeFactory = scopeFactory;

    public async Task StartCollectMetricsAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Starting metric collection process.");

        using var scope = _scopeFactory.CreateScope();
        var provider = scope.ServiceProvider;
        var context = provider.GetRequiredService<IApplicationDbContext>();
        var systemProbeService = provider.GetRequiredService<ISystemProbeService>();
        var systemRepository = provider.GetRequiredService<IWindowsMetricRepository>();

        var machineNames = await context.WindowsServers
            .Select(si => si.MachineName)
            .Where(m => !string.IsNullOrWhiteSpace(m))
            .ToListAsync(cancellationToken);

        if (!machineNames.Any())
        {
            _logger.LogWarning("No system informations found in the database. Metric collection aborted.");
            return;
        }

        var tasks = machineNames.Select(async machineName =>
        {
            try
            {
                var metric = await systemProbeService.CollectSystemMetricsAsync(machineName!, cancellationToken);
                var systemInfo = await context.WindowsServers
                    .FirstOrDefaultAsync(si => si.MachineName == machineName, cancellationToken);

                if (systemInfo != null)
                {
                    metric.WindowsId = systemInfo.Id;
                    await systemRepository.CreateSystemMetricAsync(metric, cancellationToken);
                    _logger.LogInformation("Collected and stored metrics for machine: {MachineName}", machineName);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to collect metrics for {MachineName}", machineName);
            }
        });

        await Task.WhenAll(tasks);

        _logger.LogInformation("Metric collection cycle completed for {Count} systems.", machineNames.Count);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await StartCollectMetricsAsync(stoppingToken);
            await Task.Delay(TimeSpan.FromSeconds(15), stoppingToken); // configurable interval
        }
    }
}