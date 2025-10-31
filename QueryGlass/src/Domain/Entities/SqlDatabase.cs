namespace QueryGlass.Domain.Entities;

public class SqlDatabase : BaseEntity<Guid>, IAuditableEntity
{
    public string? Name { get; set; }
    public double SizeMB { get; set; }
    public string? Status { get; set; }
    public DateTimeOffset LastBackup { get; set; }
    public bool IsSystemDatabase { get; set; }
    public Guid SqlServerInstanceId { get; set; }
    public SqlServerInstance SqlServerInstance { get; set; } = null!;
    public DateTimeOffset Created { get; set; }
    public DateTimeOffset LastModified { get; set; }
    public ICollection<SqlDatabaseMetric> SqlDatabaseMetrics { get; set; } = [];
}
