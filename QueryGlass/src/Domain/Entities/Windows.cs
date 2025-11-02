namespace QueryGlass.Domain.Entities;

public class WindowsServer : BaseAuditableEntity
{
    public string? MachineName { get; set; }
    public string? OSVersion { get; set; }
    public ICollection<SystemMetric> Metrics { get; set; } = [];
    public ICollection<SystemHealth> SystemHealths { get; set; } = [];
    public ICollection<SqlServerInstance> SqlServers { get; set; } = [];
}
