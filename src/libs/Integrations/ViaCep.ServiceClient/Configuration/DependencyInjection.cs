using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;

namespace ViaCep.ServiceClient.Configuration;

//For Resilience-related further explanation, refer to:
//https://devblogs.microsoft.com/dotnet/building-resilient-cloud-services-with-dotnet-8/
public static class DependencyInjection
{
	public static IServiceCollection AddViaCepIntegrationServiceClient(
		this IServiceCollection services,
		IConfiguration configuration)
	{
		var settings = configuration.GetSection("ServiceIntegrations:ViaCep").Get<ViaCepIntegrationSettings>()!;
		services.AddSingleton(settings);

		services.AddHttpClient<IViaCepServiceClient, ViaCepServiceClient>(config =>
		{
			config.BaseAddress = new Uri(settings.Uri);
			config.Timeout = TimeSpan.FromSeconds(settings.RequestTimeoutInSeconds);
		})
		.AddStandardResilienceHandler(options =>
		{
			//More on https://devblogs.microsoft.com/dotnet/building-resilient-cloud-services-with-dotnet-8/#standard-resilience-pipeline
			options.Retry = new Microsoft.Extensions.Http.Resilience.HttpRetryStrategyOptions()
			{
				MaxRetryAttempts = 5,
				BackoffType = Polly.DelayBackoffType.Constant,
				Delay = TimeSpan.FromSeconds(2),
				Name = "Custom ViaCep API retry strategy",
				OnRetry = (responseMessage) => {
					Console.WriteLine($"Failure while attempting to make a request to ViaCep API");
					Console.WriteLine(responseMessage.Outcome!.Exception?.GetType()?.FullName ?? "No exception was captured");

					return ValueTask.CompletedTask;
				},
				UseJitter = true,
				//This is just an example of how it is possible to customize specific situations to be handled
				ShouldHandle = (response) => ValueTask.FromResult(
					new System.Net.HttpStatusCode[] {
						System.Net.HttpStatusCode.NotFound,
						System.Net.HttpStatusCode.ServiceUnavailable }
					.Contains(response.Outcome!.Result!.StatusCode))
			};
		});

		return services;
	}
}