using Application.Core.Contracts.Messaging;
using Application.Core.Contracts.Persistence;
using Application.Core.Contracts.Repository.Caching;
using Application.Core.Contracts.Repository.Prospecting;
using Application.Core.Contracts.Repository.Security;
using Application.Core.Contracts.Repository.Security.Auditing;
using Application.Core.Contracts.Repository.Security.OneTimePassword;
using Application.Core.Contracts.Repository.UnitOfWork;
using Infrastructure.EventDispatching;
using Infrastructure.Messaging;
using Infrastructure.Persistence;
using Infrastructure.Repository.Caching;
using Infrastructure.Repository.Prospecting;
using Infrastructure.Repository.Security;
using Infrastructure.Repository.Security.Auditing;
using Infrastructure.Repository.Security.OneTimePassword;
using Infrastructure.Repository.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Events.EventDispatching;
using Shared.Settings;

namespace Infrastructure.Configuration;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        => services.AddDataSource(configuration)
                   .AddRepository(configuration)
                   .AddEventDispatcher(configuration)
                   .AddMessageBusHelper(configuration);

    private static IServiceCollection AddDataSource(this IServiceCollection services, IConfiguration configuration)
    {
        var dataSourceSettings = configuration.GetSection(nameof(DataSourceSettings)).Get<DataSourceSettings>()!;
        services.AddSingleton(dataSourceSettings);
        services.AddTransient<AppendAuditEntryInterceptor>();

        services.AddDbContext<LeadManagerDbContext>((serviceProvider, config) =>
        {
            config.UseSqlServer(
                    dataSourceSettings.ConnectionString,
                    sqlOptions =>
                    {
                        sqlOptions.EnableRetryOnFailure(dataSourceSettings.RetryOperationMaxCount);
                    })
                 .AddInterceptors(serviceProvider.GetRequiredService<AppendAuditEntryInterceptor>());
                
        });

        services.AddScoped<ILeadManagerDbContext, LeadManagerDbContext>();

        return services;
    }

    private static IServiceCollection AddRepository(this IServiceCollection services, IConfiguration configuration)
        => services
            .AddSingleton(services => configuration.GetSection($"{nameof(CachingPoliciesSettings)}:{nameof(LeadsCachingPolicy)}").Get<LeadsCachingPolicy>()!)
            .AddSingleton(services => configuration.GetSection($"{nameof(CachingPoliciesSettings)}:{nameof(AddressesCachingPolicy)}").Get<AddressesCachingPolicy>()!)
            .AddSingleton(services => configuration.GetSection($"{nameof(CachingPoliciesSettings)}:{nameof(OneTimePasswordCachingPolicy)}").Get<OneTimePasswordCachingPolicy>()!)
            .AddScoped<IUnitOfWork, UnitOfWork>()
            .AddScoped<ILeadRepository, LeadRepository>()
            .Decorate<ILeadRepository, CachingLeadRepository>()
            .AddScoped<ICachingLeadRepository, CachingLeadRepository>()
            .AddScoped<IOneTimePasswordRepository, OneTimePasswordRepository>()
            .AddScoped<IAuditingRepository, AuditingRepository>();

    private static IServiceCollection AddEventDispatcher(this IServiceCollection services, IConfiguration configuration)
        => services.AddScoped<IEventDispatching, EventDispatcher>();

    private static IServiceCollection AddMessageBusHelper(this IServiceCollection services, IConfiguration configuration)
        => services.AddSingleton<IMessageBusHelper, MessageBusHelper>();
}