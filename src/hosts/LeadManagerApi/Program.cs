using Application.Core.Configuration;
using CrossCutting.Messaging.RabbitMq.Configuration;
using HealthChecks.UI.Client;
using Infrastructure.Configuration;
using LeadManagerApi.Core.ApiFeatures;
using LeadManagerApi.Core.Configuration;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using SwaggerThemes;
using static Application.Security.LeadManagerSecurityConfiguration;

namespace LeadManagerApi;

public class Program
{
    public static void Main(string[] args)
    {
        //Services configuration
        var builder = WebApplication.CreateBuilder(args);

		builder.Services.AddApiServices(builder.Configuration);
		builder.Services.AddAggregatedTelemetry(builder.Configuration, builder.Environment);

		builder.Services.AddApplicationServices(builder.Configuration, builder.Environment);
        builder.Services.AddInfrastructureServices(builder.Configuration);

        var app = builder.Build();

        //Request pipeline configuration
        app.UseCors(Policies.LeadManagerCorsPolicy);

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(Theme.UniversalDark);
			//https://www.reddit.com/r/dotnet/comments/18m2wgx/swagger_theme_changer/
			//https://github.com/oqo0/swagger-themes/tree/main
		}

		app.UseHttpsRedirection();

        app.MapHealthChecks("/healthz", new HealthCheckOptions
        {
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });

        app.UseAuthentication();

		//Keep it commented out for demonstration purposes of how to apply in projects.
		//app.UseOutputCache();

		app.UseAuthorization();

		app.MapControllers();

        app.UseDataSourceInitialization();
        app.UseMessageBusInitialization();

        app.UseMiddleware<RequestHandlingMiddleware>();

        app.Run();
    }
}
