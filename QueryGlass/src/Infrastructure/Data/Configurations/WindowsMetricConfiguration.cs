using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QueryGlass.Domain.Entities;

namespace QueryGlass.Infrastructure.Data.Configurations;

internal sealed class SystemMetricConfiguration : IEntityTypeConfiguration<SystemMetric>
{
    public void Configure(EntityTypeBuilder<SystemMetric> builder)
    {
        builder.Property(x => x.Id).IsRequired().ValueGeneratedOnAdd();

        builder.HasOne(x => x.Windows)
            .WithMany(x => x.Metrics)
            .HasForeignKey(x => x.WindowsId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.OwnsOne(x => x.CpuDetail);
        builder.OwnsOne(x => x.MemoryDetail);

        builder.HasMany(x => x.DiskDetails)
            .WithOne(x => x.WindowsMetric)
            .HasForeignKey(c => c.WindowsMetricId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.NetworkDetails)
            .WithOne(x => x.WindowsMetric)
            .HasForeignKey(x => x.WindowsMetricId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
