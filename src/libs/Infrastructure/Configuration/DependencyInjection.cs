using Application.Contracts.Persistence;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Settings;

namespace Infrastructure.Configuration;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        var settings = new DataSourceSettings();
        configuration.Bind(nameof(DataSourceSettings), settings);

        services.AddDbContext<LeadManagerDbContext>(config =>
        {
            config.UseSqlServer(
                settings.ConnectionString,
                sqlOptions =>
                {
                    sqlOptions.EnableRetryOnFailure(settings.RetryOperationMaxCount);
                });
        });

        services.AddScoped<ILeadManagerDbContext>(
            services => services.GetRequiredService<LeadManagerDbContext>()
        );

        return services;
    }
}