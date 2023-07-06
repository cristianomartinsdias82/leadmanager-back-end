using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly.Extensions.Http;
using Polly;

namespace ViaCep.ServiceClient.Configuration
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddViaCepIntegrationServiceClient(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var settings = new ViaCepIntegrationSettings();
            configuration.Bind("ServiceIntegrations:ViaCep", settings);
            services.AddSingleton(settings);

            services.AddHttpClient<IViaCepServiceClient, ViaCepServiceClient>(config =>
            {
                config.BaseAddress = new Uri(settings.Uri);
                config.Timeout = TimeSpan.FromSeconds(settings.RequestTimeoutInSeconds);
            })
            .SetHandlerLifetime(TimeSpan.FromMinutes(5)) // <<< https://learn.microsoft.com/en-us/dotnet/architecture/microservices/implement-resilient-applications/use-httpclientfactory-to-implement-resilient-http-requests#httpclient-lifetimes
            .AddPolicyHandler(GetRetryPolicy())
            .AddPolicyHandler(GetCircuitBreakerPolicy())
            .AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(2));

            return services;
        }

        //https://learn.microsoft.com/en-us/dotnet/architecture/microservices/implement-resilient-applications/implement-http-call-retries-exponential-backoff-polly
        //https://github.com/App-vNext/Polly/wiki/Retry-with-jitter
        private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
            => HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
                .WaitAndRetryAsync(5, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)) +
                                                      TimeSpan.FromMilliseconds(Random.Shared.Next(0, 1734)));

        //https://learn.microsoft.com/en-us/dotnet/architecture/microservices/implement-resilient-applications/implement-circuit-breaker-pattern
        private static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
            => HttpPolicyExtensions
                .HandleTransientHttpError()
                .CircuitBreakerAsync(5, TimeSpan.FromSeconds(20));
    }
}