namespace QueryGlass.Domain.Entities;

public class DatabaseMigrationHistory : BaseAuditableEntity
{
    public string? FromDatabase { get; set; }
    public string? ToDatabase { get; set; }
    public string? Status { get; set; } // e.g. "Success", "Failed"
    public string? Message { get; set; } // Error or summary log
}
