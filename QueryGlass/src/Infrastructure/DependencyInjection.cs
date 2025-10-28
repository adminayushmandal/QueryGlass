using System.Globalization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
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
using Serilog.Events;
using Serilog.Formatting.Json;
using Serilog;


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
            .AddIdentityCore<ApplicationUser>()
            .AddRoles<ApplicationRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddApiEndpoints();

        builder.Services.AddSingleton(TimeProvider.System);
        builder.Services.AddTransient<IIdentityService, IdentityService>();
        builder.Services.AddScoped<ISystemInfoRepository, SystemInfoRepository>();
        builder.Services.AddScoped<ISystemMetrcRepository, SystemMetricRepository>();
        if (OperatingSystem.IsWindows())
        {
            builder.Services.AddScoped<ISystemProbeService, SystemProbeService>();
            builder.Services.AddHostedService<SystemMetricWorker>();
        }

        builder.Services.AddAuthentication()
        .AddBearerToken(IdentityConstants.BearerScheme);

        builder.Services.AddAuthorization(options =>
            options.AddPolicy(Policies.CanPurge, policy => policy.RequireRole(Roles.Administrator)));

        builder.Services.AddSerilog((sp, opt) =>
        {
            var scope = sp.CreateAsyncScope();
            var provider = scope.ServiceProvider;
            var environment = provider.GetRequiredService<IWebHostEnvironment>();

            opt.MinimumLevel.Override("Microsoft", LogEventLevel.Warning);
            opt.MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Information);
            opt.MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information);
            opt.MinimumLevel.Override("Microsoft.AspNetCore.SpaProxy", LogEventLevel.Information);

            if (environment.IsDevelopment())
            {
                opt.WriteTo.Console();
            }
            else
            {
                var path = Path.Combine(AppContext.BaseDirectory, "logs", "log-.json");
                opt.Enrich.WithProperty("QueryGlass", null, true).Filter.ByExcluding(x => x.Level == LogEventLevel.Information);
                opt.WriteTo.File(path: path, rollingInterval: RollingInterval.Day, formatter: new JsonFormatter(renderMessage: true, closingDelimiter: ",", formatProvider: CultureInfo.InvariantCulture));
            }
        });
    }
}
