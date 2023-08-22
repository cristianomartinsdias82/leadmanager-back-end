using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Configuration;

public static class HostExtensions
{
    public static IHost UseDataSourceInitialization(this IHost app)
    {
        using var scope = app.Services.CreateScope();
        var logger = scope.ServiceProvider.GetService<ILogger<IHost>>();
        var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        if (bool.TryParse(config["RunDatabaseMigrations"], out var execute) && execute)
        {
            try
            {
                var db = scope.ServiceProvider.GetRequiredService<LeadManagerDbContext>();
                db.Database.Migrate();

                logger?.LogInformation("Database migration executed successfully.");
            }
            catch (Exception exc)
            {
                logger?.LogError(exc, "Database migration error!");
            }
        }

        return app;
    }
}