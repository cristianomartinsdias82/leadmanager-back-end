﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
		services
			.AddSingleton(settings)
			.AddScoped<ViaCepServiceClientRequestAuditingHandler>()
			.AddHttpClient<IViaCepServiceClient, ViaCepServiceClient>(config =>
			{
				config.BaseAddress = new Uri(settings.Uri);
				config.Timeout = TimeSpan.FromSeconds(settings.RequestTimeoutInSeconds);
			})
			.AddHttpMessageHandler<ViaCepServiceClientRequestAuditingHandler>()
			.AddStandardResilienceHandler();
		//Down below is an example of how to customize the resilience strategy for specific situations
		//.AddStandardResilienceHandler(options =>
		//{
		//	//More on https://devblogs.microsoft.com/dotnet/building-resilient-cloud-services-with-dotnet-8/#standard-resilience-pipeline
		//	options.Retry = new Microsoft.Extensions.Http.Resilience.HttpRetryStrategyOptions()
		//	{
		//		MaxRetryAttempts = 5,
		//		BackoffType = Polly.DelayBackoffType.Constant,
		//		Delay = TimeSpan.FromSeconds(2),
		//		Name = "Custom ViaCep API retry strategy",
		//		OnRetry = (responseMessage) => {
		//			Console.WriteLine($"Failure while attempting to make a request to ViaCep API");
		//			Console.WriteLine(responseMessage.Outcome!.Exception?.GetType()?.FullName ?? "No exception was captured");

		//			return ValueTask.CompletedTask;
		//		},
		//		UseJitter = true,
				
		//		ShouldHandle = (response) => ValueTask.FromResult(
		//			new System.Net.HttpStatusCode[] {
		//				System.Net.HttpStatusCode.NotFound,
		//				System.Net.HttpStatusCode.ServiceUnavailable }
		//			.Contains(response.Outcome!.Result!.StatusCode))
		//	};
		//});

		return services;
	}
}