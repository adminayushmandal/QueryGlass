using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QueryGlass.Domain.Entities;

namespace QueryGlass.Infrastructure.Data.Configurations;

internal sealed class SqlDatabaseMetricConfiguration : IEntityTypeConfiguration<SqlDatabaseMetric>
{
    public void Configure(EntityTypeBuilder<SqlDatabaseMetric> builder)
    {
        builder.Property(x => x.Id).IsRequired().ValueGeneratedOnAdd();

        builder.Property(x => x.DataFileSizeMB).HasPrecision(10, 2);
        builder.Property(x => x.LogFileSizeMB).HasPrecision(10, 2);
        builder.Property(x => x.IndexFragmentationPercent).HasPrecision(10, 2);

        builder.HasOne(x => x.SqlDatabase)
        .WithMany(x => x.SqlDatabaseMetrics)
        .HasForeignKey(x => x.SqlDatabaseId)
        .OnDelete(DeleteBehavior.NoAction);
    }
}
