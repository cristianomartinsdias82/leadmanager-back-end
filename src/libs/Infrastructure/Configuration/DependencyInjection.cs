using Application.Core.Contracts.Messaging;
using Application.Core.Contracts.Persistence;
using Application.Core.Contracts.Reporting;
using Application.Core.Contracts.Repository.Caching;
using Application.Core.Contracts.Repository.Prospecting;
using Application.Core.Contracts.Repository.Security.Auditing;
using Application.Core.Contracts.Repository.Security.OneTimePassword;
using Application.Core.Contracts.Repository.UnitOfWork;
using Application.Reporting.Core;
using Infrastructure.EventDispatching;
using Infrastructure.Messaging;
using Infrastructure.Persistence;
using Infrastructure.Reporting.LeadsList.AllFormats;
using Infrastructure.Reporting.LeadsList.Csv;
using Infrastructure.Reporting.LeadsList.Parquet;
using Infrastructure.Reporting.LeadsList.Pdf;
using Infrastructure.Reporting.UsersActions.AllFormats;
using Infrastructure.Reporting.UsersActions.Csv;
using Infrastructure.Reporting.UsersActions.Parquet;
using Infrastructure.Reporting.UsersActions.Pdf;
using Infrastructure.Repository.Caching;
using Infrastructure.Repository.Prospecting;
using Infrastructure.Repository.Security.Auditing;
using Infrastructure.Repository.Security.OneTimePassword;
using Infrastructure.Repository.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using Shared.Events.EventDispatching;
using Shared.Exportation;
using Shared.Settings;
using System.Data.Common;

namespace Infrastructure.Configuration;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        => services.AddDataSource(configuration)
                   .AddRepository(configuration)
                   .AddEventDispatcher()
                   .AddMessageBusHelper()
				   .AddReportingGenerationServices();


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

		DbProviderFactories.RegisterFactory(dataSourceSettings.ProviderName, Microsoft.Data.SqlClient.SqlClientFactory.Instance);

		return services;
    }

    public static TracerProviderBuilder AddInfrastructureTracing(this TracerProviderBuilder tracerProviderBuilder)
        => AddDataSourceTracing(tracerProviderBuilder);

	private static TracerProviderBuilder AddDataSourceTracing(this TracerProviderBuilder tracerProviderBuilder)
    {
		//https://github.com/open-telemetry/opentelemetry-dotnet-contrib/blob/main/src/OpenTelemetry.Instrumentation.EntityFrameworkCore/README.md

		//services.Configure<EntityFrameworkInstrumentationOptions>(options =>
		return tracerProviderBuilder
			.AddEntityFrameworkCoreInstrumentation(options =>
		    {
			    options.EnrichWithIDbCommand = (activity, command) =>
			    {
				    activity.DisplayName = "Database activity";
					activity.SetTag("db.sqlstatement", command.CommandText);
					activity.SetTag("db.commandtype", command.CommandType);
				};

			    //Filters!
			    //options.Filter = (providerName, command) =>
			    //{
			    //    return command.CommandType == CommandType.StoredProcedure;
			    //};
		    })
			.AddConsoleExporter();
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

    private static IServiceCollection AddEventDispatcher(this IServiceCollection services)
        => services.AddScoped<IEventDispatching, EventDispatcher>();

    private static IServiceCollection AddMessageBusHelper(this IServiceCollection services)
        => services.AddSingleton<IMessageBusHelper, MessageBusHelper>();

	private static IServiceCollection AddReportingGenerationServices(this IServiceCollection services)
	{
		services.AddSingleton<Func<ReportGenerationFeatures, ExportFormats, IReportGeneration>>((feature, format) =>
		{
			return feature switch
			{
				ReportGenerationFeatures.LeadsList => format switch
				{
					ExportFormats.Pdf => new PdfFormatLeadsListReportGenerator(),
					ExportFormats.Csv => new CsvFormatLeadsListReportGenerator(),
					ExportFormats.Parquet => new ParquetFormatLeadsListReportGenerator(),
					ExportFormats.All => new AllFormatsLeadsListReportGenerator(),
					_ => throw new NotImplementedException()
				},
				ReportGenerationFeatures.UsersActions => format switch
				{
					ExportFormats.Pdf => new PdfFormatUsersActionsReportGenerator(),
					ExportFormats.Csv => new CsvFormatUsersActionsReportGenerator(),
					ExportFormats.Parquet => new ParquetFormatUsersActionsReportGenerator(),
					ExportFormats.All => new AllFormatsUsersActionsReportGenerator(),
					_ => throw new NotImplementedException()
				},
				_ => throw new NotImplementedException()
			};
		});

		return services;
	}
}