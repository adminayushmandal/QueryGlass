using Microsoft.Extensions.Hosting;

namespace QueryGlass.Application.Common.Interfaces;

public interface ISystemMetricWorker: IHostedService, IDisposable
{
    Task StartCollectMetricsAsync(CancellationToken cancellationToken = default);
}
