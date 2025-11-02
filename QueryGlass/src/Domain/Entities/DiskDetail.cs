namespace QueryGlass.Domain.Entities;

public class DiskDetail : BaseEntity<Guid>, IAuditableEntity
{
    public Guid WindowsMetricId { get; set; }
    public SystemMetric WindowsMetric { get; set; } = null!;
    public string? DriveLetter { get; set; }
    public double? DiskReadSpeedMBps { get; set; }
    public double? DiskWriteSpeedMBps { get; set; }
    public int? DiskIOPS { get; set; }
    public double? DiskFreeSpaceGB { get; set; }
    public double? DiskTotalSpaceGB { get; set; }
    public DateTimeOffset Created { get; set; }
    public DateTimeOffset LastModified { get; set; }
}
