using Microsoft.Extensions.Logging;
using QueryGlass.Application.Common.Interfaces;
using QueryGlass.Domain.Entities;


namespace QueryGlass.Application.SqlServer.Commands.AddNewSqlInstance;

public record AddNewSqlInstanceCommand(string ConnectionString) : IRequest<Result>;

internal class AddNewSqlInstanceCommandHandler(ISqlServerRepository repository, ISqlConnectionValidatorService validatorService, ILogger<AddNewSqlInstanceCommandHandler> logger) : IRequestHandler<AddNewSqlInstanceCommand, Result>
{
    private readonly ISqlServerRepository _repository = repository;
    private readonly ILogger<AddNewSqlInstanceCommandHandler> _logger = logger;

    public async Task<Result> Handle(AddNewSqlInstanceCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Validating SQL Server instance connection...");

        var validation = await validatorService.ValidateInstanceAsync(request.ConnectionString);
        if (!validation.IsValid)
        {
            _logger.LogWarning("SQL instance connection failed: {Error}", validation.Error);
            return Result.Failure([$"Connection failed: {validation.Error}"]);
        }
        _logger.LogInformation("Connection successful. Instance: {InstanceName}", validation.InstanceName);

        var newInstance = new SqlServerInstance
        {
            InstanceName = validation.InstanceName,
            Version = validation.Version,
            ConnectionString = request.ConnectionString,
            IsConnected = true,
            IsDefault = false,
            Created = DateTimeOffset.UtcNow,
            LastModified = DateTimeOffset.UtcNow
        };

        try
        {
            await _repository.AddInstanceAsync(newInstance, cancellationToken);
            _logger.LogInformation("SQL Server instance {InstanceName} added successfully.", newInstance.InstanceName);

            return Result.Failure([$"SQL Server instance '{newInstance.InstanceName}' added successfull"]);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while saving SQL Server instance.");
            return Result.Failure(["Failed to add SQL Server instance. "]);
        }
    }
}
