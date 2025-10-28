using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QueryGlass.Application.Common.Interfaces;
using QueryGlass.Domain.Entities;
using QueryGlass.Infrastructure.Data;

namespace QueryGlass.Infrastructure.Repositories;

internal sealed class SystemInfoRepository(ILogger<SystemInfoRepository> logger, ApplicationDbContext context) : ISystemInfoRepository
{
    private readonly ILogger<SystemInfoRepository> _logger = logger;
    private readonly ApplicationDbContext _context = context;

    public async Task<SystemInfo?> CreateAsync(SystemInfo systemInfo, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Start adding new machine '{machineName}'", systemInfo.MachineName);

        if (string.IsNullOrEmpty(systemInfo.MachineName))
        {
            throw new InvalidOperationException("Machine name cannot be null.");
        }

        if (string.IsNullOrEmpty(systemInfo.OSVersion))
        {
            throw new InvalidOperationException("Machine's operating system cannot be null or empty.");
        }

        await _context.SystemInformations.AddAsync(systemInfo, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Machine '{machineName}' is added successfully.", systemInfo.MachineName);

        return systemInfo;
    }

    public async Task<bool> DeleteAsync(Guid systemId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deleting system info of id '{systemId}'", systemId);
        var entity = await _context.SystemInformations.FirstOrDefaultAsync(x => x.Id == systemId, cancellationToken: cancellationToken)
            ?? throw new KeyNotFoundException($"Entity with id '{systemId}' not found.");

        _context.SystemInformations.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("System info is deleted successfully.");
        return true;
    }

    public async Task<SystemInfo?> GetSystemInfoByIdAsync(Guid systemId, CancellationToken cancellationToken = default)
        => await _context.SystemInformations.FirstOrDefaultAsync(x => x.Id == systemId, cancellationToken)
        ?? throw new KeyNotFoundException($"System info with d '{systemId}' not founded.");

    public async Task<IEnumerable<SystemInfo>> GetSystemsAsync(CancellationToken cancellationToken = default)
        => await _context.SystemInformations.AsNoTracking().ToListAsync(cancellationToken: cancellationToken);

    public async Task<bool> IsExistAsync(string machineName, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(machineName)) throw new ArgumentNullException(nameof(machineName), "Machine name cannot be null or empty.");
        var entityExists = await _context.SystemInformations.AnyAsync(x => x.MachineName == machineName, cancellationToken);
        return entityExists;
    }

    public async Task<SystemInfo> UpdateAsync(SystemInfo systemInfo, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("System info with id '{systemId}' starts updating...", systemInfo.Id);

        var entity = await _context.SystemInformations
            .FirstOrDefaultAsync(x => x.Id == systemInfo.Id, cancellationToken: cancellationToken)
            ?? throw new KeyNotFoundException($"System info with id '{systemInfo.Id}'");

        _context.SystemInformations.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("System info with id '{systemId}' s updated.", systemInfo.Id);

        return entity;
    }
}
