using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QueryGlass.Application.Common.Interfaces;
using QueryGlass.Domain.Entities;
using QueryGlass.Infrastructure.Data;

namespace QueryGlass.Infrastructure.Repositories;

internal sealed class SqlServerRepository(ILogger<SqlServerRepository> logger, ApplicationDbContext context) : ISqlServerRepository
{
    private readonly ILogger<SqlServerRepository> _logger = logger;
    private readonly ApplicationDbContext _context = context;
    public async Task AddDatabaseMetricsAsync(SqlDatabaseMetric metric, CancellationToken cancellationToken = default)
    {
        _context.SqlDatabaseMetrics.Add(metric);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task AddInstanceAsync(SqlServerInstance instance, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Adding new sql server instance...");

        if (string.IsNullOrEmpty(instance.InstanceName)) throw new InvalidOperationException("Instance name should not be empty or null");

        _context.SqlServerInstances.Add(instance);
        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Instance is added successfully.");
    }

    public async Task AddInstanceMetricsAsync(SqlServerMetric metric, CancellationToken cancellationToken = default)
    {
        _context.SqlServerMetrics.Add(metric);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IEnumerable<SqlServerInstance>> GetAllAsync(CancellationToken cancellationToken = default)
    => await _context.SqlServerInstances
            .Include(x => x.Databases)
            .ToListAsync(cancellationToken);

    public async Task<SqlServerInstance?> GetInstanceAsync(Guid id, CancellationToken cancellationToken = default)
     => await _context.SqlServerInstances
            .Include(x => x.Databases)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task UpdateInstanceAsync(SqlServerInstance instance, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updateing instance '{instanceName}'", instance.InstanceName);
        _context.SqlServerInstances.Update(instance);
        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Sql instance is updated successfully.");
    }
}
