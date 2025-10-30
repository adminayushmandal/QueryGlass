using System.Text.RegularExpressions;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using QueryGlass.Application.Common.Interfaces;
using QueryGlass.Domain.Entities;


namespace QueryGlass.Application.SqlServer.Commands.AddNewSqlInstance;

public record AddNewSqlInstanceCommand(string ServerName) : IRequest<Result>;

internal class AddNewSqlInstanceCommandHandler(ISqlServerRepository repository, ISqlConnectionValidatorService validatorService, ILogger<AddNewSqlInstanceCommandHandler> logger, IWindowsRepository systemInfoRepository) : IRequestHandler<AddNewSqlInstanceCommand, Result>
{
    private readonly ISqlServerRepository _repository = repository;
    private readonly ILogger<AddNewSqlInstanceCommandHandler> _logger = logger;
    private readonly IWindowsRepository _systemInfoRepository = systemInfoRepository;

    public async Task<Result> Handle(AddNewSqlInstanceCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Validating SQL Server instance connection...");

        var input = request.ServerName.Trim();

        // Matches optional protocol (tcp:), machine name or IP, optional instance (\something), and optional port (,number)
        var match = Regex.Match(input, @"^(?:tcp:)?(?<machine>[^\\,]+)(?:\\(?<instance>[^,]+))?(?:,(?<port>\d+))?$", RegexOptions.IgnoreCase);

        if (!match.Success)
        {
            return Result.Failure([$"Invalid SQL Server name format: '{input}'."]);
        }

        var machineName = match.Groups["machine"].Value;
        var instanceName = match.Groups["instance"].Success ? match.Groups["instance"].Value : null;
        var port = match.Groups["port"].Success ? int.Parse(match.Groups["port"].Value) : (int?)null;


        var windowsServer = await _systemInfoRepository.GetSystemInfoByNameAsync(machineName, cancellationToken)
            ?? throw new KeyNotFoundException($"Windows server with this name '{machineName}' not found. Please add windows server first.");

        var sqlConnectionBuilder = new SqlConnectionStringBuilder
        {
            DataSource = $"{instanceName ?? machineName}",
            InitialCatalog = "master",
            IntegratedSecurity = true, // or supply credentials below
            Encrypt = false,
            TrustServerCertificate = true,
            ConnectTimeout = 10
        };

        var connectionString = sqlConnectionBuilder.ConnectionString;

        var validation = await validatorService.ValidateInstanceAsync(connectionString);
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
            ConnectionString = connectionString,
            IsConnected = true,
            IsDefault = false,
            ServerId = windowsServer.Id
        };

        try
        {
            await _repository.AddInstanceAsync(newInstance, cancellationToken);
            _logger.LogInformation("SQL Server instance {InstanceName} added successfully.", newInstance.InstanceName);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while saving SQL Server instance.");
            return Result.Failure(["Failed to add SQL Server instance. "]);
        }
    }
}
