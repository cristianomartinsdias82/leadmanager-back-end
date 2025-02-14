using Application.Core.Configuration;
using Infrastructure.Configuration;
using LeadManagerApi.Core.ApiFeatures;
using LeadManagerApi.Core.Configuration.Security;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System.Reflection;
using System.Text.Json.Serialization;

namespace LeadManagerApi.Core.Configuration;

public static class DependencyInjection
{
    public static IServiceCollection AddApiServices(
        this IServiceCollection services,
        IConfiguration configuration,
        IWebHostEnvironment hostingEnvironment)
    {
        var apiSettings = configuration.GetSection(nameof(LeadManagerApiSettings)).Get<LeadManagerApiSettings>()!;
        services.AddSingleton(apiSettings);

        //You can see how this configuration reflects in Swagger (GetLeads endpoint -> SortDirection parameter. Instead of displaying the options 0 or 1, it displays Ascending or Descending)
		services.Configure<JsonOptions>(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

		services.AddControllers(config =>
        {
            config.Filters.Add<RequiresApiKeyActionFilter>();

			//https://stackoverflow.com/questions/36413476/mvc-core-how-to-force-set-global-authorization-for-all-actions
			//var policy = new AuthorizationPolicyBuilder()
			//                    .RequireAuthenticatedUser()
			//                    .Build();
			//config.Filters.Add(new AuthorizeFilter(policy));
		});

        services.AddAuthorization(LeadManagerApiSecurityConfiguration.SetPermissionPolicies);

        services.AddCors(options => LeadManagerApiSecurityConfiguration.SetCorsPolicy(options, apiSettings));

        services.Configure<KestrelServerOptions>(options =>
        {
            var apiSettings = configuration
                                .GetSection(nameof(LeadManagerApiSettings))
                                .Get<LeadManagerApiSettings>()!;

            options.Limits.MaxRequestBodySize = apiSettings.FileUpload_MaxSizeInBytes;
        });

        services.Configure<FormOptions>(options =>
        {
            options.MultipartBodyLengthLimit = apiSettings.FileUpload_MaxSizeInBytes;
        });

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(config =>
        {
			config.OperationFilter<ApiKeyHeaderOperationFilter>();

			LeadManagerApiSecurityConfiguration.SetAuthorizationForSwagger(config);
		});

        return services;
    }

    public static IServiceCollection AddAggregatedTelemetry(
                            this IServiceCollection services,
                            IConfiguration configuration,
                            IWebHostEnvironment hostingEnvironment)
    {
		var OtlpEndpoint = new Uri(configuration["OTLP_Endpoint"]!);

		services.AddOpenTelemetry()
                .ConfigureResource(res => res
                                         .AddService
                                         ("LeadManager.Api",
                                          serviceNamespace: "LeadManagerApi",
                                          serviceVersion: Assembly.GetExecutingAssembly().GetName().Version!.ToString()
                                         )
                                         .AddAttributes(
                                            [
                                                new KeyValuePair<string,object>("service.env", hostingEnvironment.EnvironmentName)
                                                //Add other service attributes here
                                            ]
                                         )
                )
                .WithTracing(trc => trc
                         .AddConsoleExporter()
                         .AddAspNetCoreInstrumentation()
                         .AddHttpClientInstrumentation() //"This API communicates to the RiskEvaluator GRPC service. As such, you need to add telemetry IN HERE so we can correlate it with Parent Span and visualize it as a hierarchical way in Jaeger!"
                         .AddApplicationTracing(services, configuration, hostingEnvironment)
                         .AddInfrastructureTracing(services, configuration, hostingEnvironment)
                         .AddOtlpExporter(cfg => cfg.Endpoint = OtlpEndpoint /*You can configure the protocol if needed, like this: cfg.Protocol = OtlpExportProtocol.Grpc or .HttpProtoBuf*/)
                )
				.WithMetrics(mtr => mtr
					.AddConsoleExporter()
					.AddAspNetCoreInstrumentation()
					.AddHttpClientInstrumentation()
					//Metrics provided by Asp.Net Core
					.AddMeter("Microsoft.AspNetCore.Hosting")
					.AddMeter("Microsoft.AspNetCore.Server.Kestrel")
                    .AddApplicationMetric(services, configuration, hostingEnvironment)
					.AddOtlpExporter(cfg => cfg.Endpoint = OtlpEndpoint)
				    //.AddPrometheusExporter() //Necessary only when using the collector's pulling model. (The application sends data to the collector and Prometheus scrapes that data from there)
				)
			    .WithLogging(lgnBuilder => lgnBuilder
					.AddConsoleExporter()
					.AddOtlpExporter(cfg => cfg.Endpoint = OtlpEndpoint),
				                    lgnOptions => //This argument is for log enrichment and customization purposes
				                    {
				                        lgnOptions.IncludeScopes = true;
				                        lgnOptions.IncludeFormattedMessage = true;
				                        //lngOptions.ParseStateValues = true;
				                    }
                );

        return services;
	}
}
