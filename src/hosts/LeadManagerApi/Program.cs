using Application.Configuration;
using Infrastructure.Configuration;
using LeadManagerApi.ApiFeatures;
using LeadManagerApi.Configuration;

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
