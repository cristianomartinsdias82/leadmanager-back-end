using Application.Configuration;
using Infrastructure.Configuration;
using LeadManagerApi.ApiFeatures;
using LeadManagerApi.Configuration;
using CrossCutting.Messaging.RabbitMq.Configuration;

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
        app.UseMessageBusInitialization();

        app.UseMiddleware<RequestHandlingMiddleware>();

        app.Run();
    }
}
