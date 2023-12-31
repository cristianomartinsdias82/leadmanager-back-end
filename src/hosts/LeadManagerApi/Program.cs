using Application.Core.Configuration;
using CrossCutting.Messaging.RabbitMq.Configuration;
using HealthChecks.UI.Client;
using Infrastructure.Configuration;
using LeadManagerApi.Core.ApiFeatures;
using LeadManagerApi.Core.Configuration;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
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

        app.UseAuthentication();

        app.UseAuthorization();

        app.MapControllers();

        app.UseDataSourceInitialization();
        app.UseMessageBusInitialization();

        app.UseMiddleware<RequestHandlingMiddleware>();

        app.Run();
    }
}
