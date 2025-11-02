using Microsoft.Extensions.Hosting;

namespace QueryGlass.Application.Common.Interfaces;

public interface IWindowsMetricWorker: IHostedService, IDisposable
{
    Task StartCollectMetricsAsync(CancellationToken cancellationToken = default);
}
