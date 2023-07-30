using LeadManagerApi.ApiFeatures;
using Microsoft.AspNetCore.Http.Features;

namespace LeadManagerApi.Configuration
{
    public static class DependencyInjection
    {
        public const string LeadWebAppCorsPolicy = "LeadWebAppCorsPolicy";

        public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
        {
            var apiSettings = configuration.GetSection(nameof(LeadManagerApiSettings)).Get<LeadManagerApiSettings>()!;
            services.AddSingleton(apiSettings);

            services.AddControllers(config =>
            {
                config.Filters.Add<RequiresApiKeyActionFilter>();
            });

            services.Configure<FormOptions>(options =>
            {
                options.MultipartBodyLengthLimit = apiSettings.FileUpload_MaxSizeInBytes;
            });

            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(config =>
            {
                //config.OperationFilter<ApiKeyHeaderOperationFilter>(); //It is causing error in Swagger view page
            });
            services.AddCors(options =>
            {
                options.AddPolicy(LeadWebAppCorsPolicy,
                    builder =>
                    {
                        builder.WithOrigins(apiSettings.Cors_AllowedOrigins.Split(','))
                               .WithMethods("POST", "GET", "PUT", "DELETE", "HEAD", "OPTIONS")
                               .AllowAnyHeader()
                               .AllowCredentials()
                               .Build();
                    });
            });

            return services;
        }
    }
}
