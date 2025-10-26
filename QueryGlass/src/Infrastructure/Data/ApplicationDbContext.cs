using System.Reflection;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using QueryGlass.Application.Common.Interfaces;
using QueryGlass.Domain.Entities;

namespace QueryGlass.Infrastructure.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>(options), IApplicationDbContext
{
    public DbSet<TodoList> TodoLists => Set<TodoList>();

    public DbSet<TodoItem> TodoItems => Set<TodoItem>();

    public DbSet<SystemInfo> SystemInformations => Set<SystemInfo>();

    public DbSet<SystemMetric> SystemMetrics => Set<SystemMetric>();

    public DbSet<SqlServerInstance> SqlServerInstances => Set<SqlServerInstance>();

    public DbSet<SqlDatabase> SqlDatabases => Set<SqlDatabase>();

    public DbSet<SystemHealth> Healths => Set<SystemHealth>();

    public DbSet<DiskDetail> DiskDetails => Set<DiskDetail>();

    public DbSet<NetworkDetail> NetworkDetails => Set<NetworkDetail>();

    public DbSet<DatabaseMigrationHistory> DatabaseMigrationHistory => Set<DatabaseMigrationHistory>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.HasDefaultSchema("QueryGlass");
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
