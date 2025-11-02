using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QueryGlass.Domain.Entities;

namespace QueryGlass.Infrastructure.Data.Configurations;

internal sealed class SqlInstanceMetricConfiguration : IEntityTypeConfiguration<SqlServerMetric>
{
    public void Configure(EntityTypeBuilder<SqlServerMetric> builder)
    {
        builder.Property(x => x.Id).IsRequired().ValueGeneratedOnAdd();

        builder.Property(x => x.CpuUsagePercent).HasPrecision(10, 2);
        builder.Property(x => x.MemoryUsageMB).HasPrecision(10, 2);
        builder.Property(x => x.DiskIOPS).HasPrecision(10, 2);
        builder.Property(x => x.TransactionRatePerSec).HasPrecision(10, 2);

        builder.HasOne(x => x.SqlServerInstance)
        .WithMany(s => s.SqlServerMetrics)
        .HasForeignKey(fk => fk.SqlServerInstanceId)
        .OnDelete(DeleteBehavior.NoAction);
    }
}
