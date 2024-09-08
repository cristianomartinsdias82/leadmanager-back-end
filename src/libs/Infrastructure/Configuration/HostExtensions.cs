using Domain.Prospecting.Entities;
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

				logger?.LogInformation("Adding Lead sample data...");
                AddLeadSampleData(db);

				logger?.LogInformation("Lead sample data registration successful.");
			}
            catch (Exception exc)
            {
                logger?.LogError(exc, "Database migration error!");
            }
        }

        return app;
    }

    private static void AddLeadSampleData(LeadManagerDbContext dbContext)
    {
        if (dbContext.Leads.Any())
            return;

        dbContext.Leads.Add(Lead.Criar(
			"20.321.123/0001-54",
			"Lead 1",
			"04858-040",
			"Rua do Escorpião Louco",
			"Vila Parí",
            "São Paulo",
            "SP",
            "2609",
            null));

        dbContext.SaveChanges();
	}
}