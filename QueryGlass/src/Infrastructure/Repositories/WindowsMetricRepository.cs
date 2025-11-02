using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QueryGlass.Application.Common.Interfaces;
using QueryGlass.Domain.Entities;
using QueryGlass.Infrastructure.Data;

namespace QueryGlass.Infrastructure.Repositories;

internal sealed class WindowsMetricRepository(ILogger<WindowsMetricRepository> logger, ApplicationDbContext context) : IWindowsMetricRepository
{
    private readonly ILogger<WindowsMetricRepository> _logger = logger;
    private readonly ApplicationDbContext _context = context;

    public async Task<SystemMetric?> CreateSystemMetricAsync(SystemMetric systemMetric, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating new SystemMetric with ID: {SystemMetricId}", systemMetric.Id);
        await _context.SystemMetrics.AddAsync(systemMetric, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Successfully created SystemMetric with ID: {SystemMetricId}", systemMetric.Id);
        return systemMetric;
    }

    public async Task<bool> DeleteSystemMetricAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var systemMetric = await _context.SystemMetrics.FindAsync([id], cancellationToken);
        if (systemMetric == null)
        {
            _logger.LogWarning("SystemMetric with ID: {SystemMetricId} not found", id);
            return false;
        }

        _context.SystemMetrics.Remove(systemMetric);
        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Successfully deleted SystemMetric with ID: {SystemMetricId}", id);
        return true;
    }

    public async Task<IEnumerable<SystemMetric>> GetAllSystemMetricsAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Retrieving all SystemMetrics");
        var systemMetrics = await _context.SystemMetrics.ToListAsync(cancellationToken);
        _logger.LogInformation("Successfully retrieved {SystemMetricsCount} SystemMetrics", systemMetrics.Count);
        return systemMetrics;
    }

    public async Task<SystemMetric?> GetSystemMetricByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Retrieving SystemMetric with ID: {SystemMetricId}", id);
        var systemMetric = await _context.SystemMetrics.FindAsync([id], cancellationToken);
        if (systemMetric == null)
        {
            _logger.LogWarning("SystemMetric with ID: {SystemMetricId} not found", id);
        }
        else
        {
            _logger.LogInformation("Successfully retrieved SystemMetric with ID: {SystemMetricId}", id);
        }
        return systemMetric;
    }
}