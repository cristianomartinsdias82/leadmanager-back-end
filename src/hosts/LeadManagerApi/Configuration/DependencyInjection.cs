using LeadManagerApi.ApiFeatures;

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
                        builder.WithOrigins("http://localhost:4200")
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
