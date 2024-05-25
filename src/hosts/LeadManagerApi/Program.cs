using Application.Core.Configuration;
using CrossCutting.Messaging.RabbitMq.Configuration;
using HealthChecks.UI.Client;
using Infrastructure.Configuration;
using LeadManagerApi.Core.ApiFeatures;
using LeadManagerApi.Core.Configuration;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Prometheus;
using static Application.Security.LeadManagerSecurityConfiguration;

namespace LeadManagerApi;

public class Program
{
    public static void Main(string[] args)
    {
        //Services configuration
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddApiServices(builder.Configuration)
                        .AddApplicationServices(builder.Configuration)
                        .AddInfrastructureServices(builder.Configuration);

        var app = builder.Build();

        //Request pipeline configuration
        app.UseCors(Policies.LeadManagerCorsPolicy);

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.MapHealthChecks("/_health", new HealthCheckOptions
        {
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });

        app.UseHttpMetrics(options =>
        {
            options.AddCustomLabel("host", context => context.Request.Host.Host);
        });

        app.UseAuthentication();

        app.UseRouting();

        app.UseAuthorization();

        #pragma warning disable ASP0014
        app.UseEndpoints(endpoints =>
        {
            // Enable the /metrics page to export Prometheus metrics.
            // Open http://localhost:[port]/metrics to see the metrics.
            //
            // Metrics published in this sample:
            // * built-in process metrics giving basic information about the .NET runtime (enabled by default)
            // * metrics from .NET Event Counters (enabled by default, updated every 10 seconds)
            // * metrics from .NET Meters (enabled by default)
            // * metrics about requests made by registered HTTP clients used in SampleService (configured above)
            // * metrics about requests handled by the web app (configured above)
            // * ASP.NET health check statuses (configured above)
            // * custom business logic metrics published by the SampleService class
            endpoints.MapMetrics();
        });
        #pragma warning restore ASP0014

        app.MapControllers();

        app.UseDataSourceInitialization();
        app.UseMessageBusInitialization();

        app.UseMiddleware<RequestHandlingMiddleware>();

        app.Run();
    }
}
