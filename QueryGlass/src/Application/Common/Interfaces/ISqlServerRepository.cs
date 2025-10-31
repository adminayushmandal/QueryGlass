using QueryGlass.Domain.Entities;

namespace QueryGlass.Application.Common.Interfaces;

public interface ISqlServerRepository
{
    Task<SqlServerInstance?> GetInstanceAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<SqlServerInstance>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddInstanceAsync(SqlServerInstance instance, CancellationToken cancellationToken = default);
    Task UpdateInstanceAsync(SqlServerInstance instance, CancellationToken cancellationToken = default);

    // Metrics
    Task AddInstanceMetricsAsync(SqlServerMetric metric, CancellationToken cancellationToken = default);
    Task AddDatabaseMetricsAsync(SqlDatabaseMetric metric, CancellationToken cancellationToken = default);
}
