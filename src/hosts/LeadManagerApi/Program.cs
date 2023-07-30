using Application.Configuration;
using Infrastructure.Configuration;
using LeadManagerApi.ApiFeatures;
using LeadManagerApi.Configuration;
using Microsoft.AspNetCore.Server.Kestrel.Core;

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

        builder.Services.Configure<KestrelServerOptions>(options =>
        {
            var apiSettings = builder.Configuration.GetSection(nameof(LeadManagerApiSettings)).Get<LeadManagerApiSettings>()!;

            options.Limits.MaxRequestBodySize = apiSettings.FileUpload_MaxSizeInBytes;
        });

        //TODO: builder.Host.CONFIGURELOGGING()!

        var app = builder.Build();

        //Pipeline configuration
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

        app.UseMiddleware<ErrorHandlingMiddleware>();

        app.Run();
    }
}
