using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QueryGlass.Domain.Entities;

namespace QueryGlass.Infrastructure.Data.Configurations;

internal sealed class SystemInfoConfiguration : IEntityTypeConfiguration<WindowsServer>
{
    public void Configure(EntityTypeBuilder<WindowsServer> builder)
    {
        builder.Property(x => x.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Property(x => x.MachineName).IsRequired();
        builder.Property(x => x.OSVersion).IsRequired();
    }
}
