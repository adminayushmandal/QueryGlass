using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using QueryGlass.Domain.Constants;
using QueryGlass.Domain.Entities;

namespace QueryGlass.Infrastructure.Data;

public static class InitialiserExtensions
{
    public static async Task InitialiseDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var initialiser = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitialiser>();

        await initialiser.InitialiseAsync();
        await initialiser.SeedAsync();
    }
}

public class ApplicationDbContextInitialiser(ILogger<ApplicationDbContextInitialiser> logger, ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
{
    private readonly ILogger<ApplicationDbContextInitialiser> _logger = logger;
    private readonly ApplicationDbContext _context = context;
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly RoleManager<ApplicationRole> _roleManager = roleManager;

    public async Task InitialiseAsync()
    {
        try
        {
            // See https://jasontaylor.dev/ef-core-database-initialisation-strategies
            await _context.Database.EnsureDeletedAsync();
            await _context.Database.EnsureCreatedAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while initialising the database.");
            throw;
        }
    }

    public async Task SeedAsync()
    {
        try
        {
            await TrySeedAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }
    }

    public async Task TrySeedAsync()
    {
        try
        {
            await SeedRolesAsync();
            await SeedAdminUserAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occured during seeding.");
            throw;
        }
    }

    public async Task SeedRolesAsync()
    {
        _logger.LogInformation("Start seeding application roles...");
        try
        {

            var adminRole = new ApplicationRole(Roles.Administrator, "Full system access.");
            var operatorRole = new ApplicationRole(Roles.Operator, "Can monitor and manage services");
            var viewerRole = new ApplicationRole(Roles.Viewer, "Read-only monitoring access");
            var dbaRole = new ApplicationRole(Roles.DBA, "Database administrator with DB control privileges.");

            List<ApplicationRole> roles = [adminRole, operatorRole, viewerRole, dbaRole];

            foreach (var role in roles)
            {
                if (!string.IsNullOrEmpty(role.Name) && !await _roleManager.RoleExistsAsync(role.Name))
                {
                    var result = await _roleManager.CreateAsync(role);

                    if (result.Succeeded)
                    {
                        _logger.LogInformation("Rolen '{roleName}' successfully added", role.Name);
                    }
                    else
                    {
                        _logger.LogWarning("Failed to create role '{roleName}'\nError '{errorReason}'", role.Name, string.Join(", ", result.Errors.Select(x => x.Description)));
                    }
                }
            }
            _logger.LogInformation("Role seeding completed successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occured during the seeding roles.");
            throw;
        }
    }

    private async Task SeedAdminUserAsync()
    {
        _logger.LogInformation("Seeding admin user...");
        var user = new ApplicationUser
        {
            DisplayName = "Ayush Krishan Mandal",
            Email = "ayush@localhost",
            UserName = "ayush@localhost",
        };

        try
        {
            if (_userManager.Users.All(x => x.UserName != user.UserName))
            {
                var result = await _userManager.CreateAsync(user, "Test@123");

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, Roles.Administrator);
                    _logger.LogInformation("User '{email}' is created.", user.Email);
                }
                else
                {
                    _logger.LogWarning("Failed to seed user '{email}'", user.Email);
                }
            }
            _logger.LogInformation("User '{email}' is created successfully.", user.Email);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occured during the seed user");
            throw;
        }
    }
}
