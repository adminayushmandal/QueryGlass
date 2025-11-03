using QueryGlass.Application.Common.Interfaces;
using QueryGlass.Application.Common.Models;
using QueryGlass.Domain.Entities;

namespace QueryGlass.Infrastructure.Services;

internal sealed class SqlServerInstanceService(ISqlServerRepository sqlServerRepository)
{
    private readonly ISqlServerRepository _repository = sqlServerRepository;
    public async Task<Result> AddInstanceAsync(string connectionString, CancellationToken cancellationToken = default)
    {
        var validation = await SqlConnectionValidator.ValidateInstanceAsync(connectionString);
        if (!validation.IsValid)
            return Result.Failure([$"Connection failed: {validation.Error}"]);

        var instance = new SqlServerInstance
        {
            InstanceName = validation.InstanceName,
            Version = validation.Version,
            ConnectionString = connectionString,
            IsConnected = true,
            Created = DateTimeOffset.UtcNow,
            LastModified = DateTimeOffset.UtcNow
        };

        await _repository.AddInstanceAsync(instance, cancellationToken);
        return Result.Success();
    }

    public async Task<Result> AddDatabaseAsync(Guid instanceId, string databaseName, CancellationToken cancellationToken = default)
    {
        var instance = await _repository.GetInstanceAsync(instanceId, cancellationToken);
        if (instance == null)
            return Result.Failure(["SQL Server instance not found."]);

        var validation = await SqlConnectionValidator.ValidateDatabaseAsync(instance.ConnectionString!, databaseName);
        if (!validation.Exists)
            return Result.Failure([$"Database '{databaseName}' does not exist or cannot be accessed. {validation.Error}"]);

        var database = new SqlDatabase
        {
            Name = databaseName,
            SqlServerInstanceId = instance.Id,
            Created = DateTimeOffset.UtcNow,
            LastModified = DateTimeOffset.UtcNow
        };

        await _repository.AddInstanceAsync(instance, cancellationToken);
        return Result.Success();
    }
}
