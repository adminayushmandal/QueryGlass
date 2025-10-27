using System.Runtime.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using QueryGlass.Application.Common.Interfaces;
using QueryGlass.Domain.Constants;
using QueryGlass.Domain.Entities;
using QueryGlass.Infrastructure.Data;
using QueryGlass.Infrastructure.Data.Interceptors;
using QueryGlass.Infrastructure.Identity;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static void AddInfrastructureServices(this IHostApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString("QueryGlassDb");
        Guard.Against.Null(connectionString, message: "Connection string 'QueryGlassDb' not found.");

        builder.Services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();
        builder.Services.AddScoped<ISaveChangesInterceptor, DispatchDomainEventsInterceptor>();

        builder.Services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
            options.UseNpgsql(connectionString);
            options.ConfigureWarnings(warnings => warnings.Ignore(RelationalEventId.PendingModelChangesWarning));
        });


        builder.Services.AddScoped<IApplicationDbContext>(provider =>
            provider.GetRequiredService<ApplicationDbContext>());

        builder.Services.AddScoped<ApplicationDbContextInitialiser>();

        builder.Services
            .AddDefaultIdentity<ApplicationUser>()
            .AddRoles<ApplicationRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>();

        builder.Services.AddSingleton(TimeProvider.System);
        builder.Services.AddTransient<IIdentityService, IdentityService>();
        builder.Services.AddScoped<ISystemInfoRepository, SystemInfoRepository>();
        if (OperatingSystem.IsWindows())
        {
            builder.Services.AddScoped<ISystemProbeService, SystemProbeService>();
        }

        builder.Services.AddAuthorization(options =>
            options.AddPolicy(Policies.CanPurge, policy => policy.RequireRole(Roles.Administrator)));
    }
}
