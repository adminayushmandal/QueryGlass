using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QueryGlass.Domain.Entities;

namespace QueryGlass.Infrastructure.Data.Configurations;

internal sealed class SystemMetricConfiguration : IEntityTypeConfiguration<SystemMetric>
{
    public void Configure(EntityTypeBuilder<SystemMetric> builder)
    {
        builder.Property(x => x.Id).IsRequired().ValueGeneratedOnAdd();

        builder.HasOne(x => x.SystemInfo)
            .WithMany(x => x.Metrics)
            .HasForeignKey(x => x.SystemInfoId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.OwnsOne(x => x.CpuDetail);
        builder.OwnsOne(x => x.MemoryDetail);

        builder.HasMany(x => x.DiskDetails)
            .WithOne(x => x.SystemMetric)
            .HasForeignKey(c => c.SystemMetricId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasMany(x => x.NetworkDetails)
            .WithOne(x => x.SystemMetric)
            .HasForeignKey(x => x.SystemMetricId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
