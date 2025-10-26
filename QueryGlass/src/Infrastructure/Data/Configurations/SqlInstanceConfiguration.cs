using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QueryGlass.Domain.Entities;

namespace QueryGlass.Infrastructure.Data.Configurations;

internal sealed class SqlInstanceConfiguration : IEntityTypeConfiguration<SqlServerInstance>
{
    public void Configure(EntityTypeBuilder<SqlServerInstance> builder)
    {
        builder.Property(x => x.Id).IsRequired().ValueGeneratedOnAdd();

        builder.Property(x => x.InstanceName).IsRequired().HasMaxLength(256);

        builder.Property(x => x.ConnectionString).IsRequired();

        builder.Property(x => x.Version).IsRequired();
    }
}
