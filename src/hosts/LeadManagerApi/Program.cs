using Application.Configuration;
using Infrastructure.Configuration;
using LeadManagerApi.ApiFeatures;
using LeadManagerApi.Configuration;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Serilog;
using Serilog.Sinks.MSSqlServer;
using Shared.Settings;
using System.Data;
using System.Text;

namespace LeadManagerApi;

public class Program
{
    public static void Main(string[] args)
    {
        //Services configuration
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddApiServices(builder.Configuration);

        builder.Services.AddApplicationServices(builder.Configuration);

        builder.Services.AddInfrastructureServices(builder.Configuration);

        var app = builder.Build();

        //Request pipeline configuration
        app.UseCors(Configuration.DependencyInjection.LeadWebAppCorsPolicy);

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.UseDataSourceInitialization();

        app.UseMiddleware<RequestHandlingMiddleware>();

        app.Run();
    }
}
