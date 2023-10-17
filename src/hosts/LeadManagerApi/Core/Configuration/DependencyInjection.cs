using LeadManagerApi.Core.ApiFeatures;
using LeadManagerApi.Core.Configuration.Security;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Server.Kestrel.Core;

namespace LeadManagerApi.Core.Configuration;

public static class DependencyInjection
{
    public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
    {
        var apiSettings = configuration.GetSection(nameof(LeadManagerApiSettings)).Get<LeadManagerApiSettings>()!;
        services.AddSingleton(apiSettings);

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
            var apiSettings = configuration.GetSection(nameof(LeadManagerApiSettings)).Get<LeadManagerApiSettings>()!;

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
}
