using CrossCutting.Messaging.Configuration;
using CrossCutting.Messaging.Consumers.BackendServicesCommon.Diagnostics.SpanProcessors;
using CrossCutting.Messaging.RabbitMq.Configuration;
using CrossCutting.Messaging.RabbitMq.Diagnostics;
using CrossCutting.Serialization.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System.Reflection;

namespace CrossCutting.Messaging.Consumers.BackendServices.Common;

public class ConsumerServiceHostBuilder
{
	private readonly string[] _args;
	private List<Action<IServiceCollection>> _addHostedService;

	private ConsumerServiceHostBuilder(string[] args)
	{
		_args = args;
		_addHostedService = new List<Action<IServiceCollection>>();
	}

	public static ConsumerServiceHostBuilder New(string[] args) => new(args);

	public ConsumerServiceHostBuilder WithHostedService<THostedService>() where THostedService : class, IHostedService
	{
		_addHostedService.Add(services => services.AddHostedService<THostedService>());

		return this;
	}

	public IHost Build()
		=> Host
				.CreateDefaultBuilder(_args)
				.ConfigureServices((context, services) =>
				{
					var OtlpEndpoint = new Uri(context.Configuration["OTLP_Endpoint"]!);

					var configuration = services.BuildServiceProvider().GetRequiredService<IConfiguration>();

					services
						.AddSingleton((_) => TimeProvider.System)
						.AddSerialization(configuration)
						.AddMessageBus(configuration)
						.ConfigureOpenTelemetryTracerProvider(tracerProviderBuilder =>
						{
							//Register Span Processors that, for example, set tags from Baggage for context propagation purposes
							//(see SpanProcessors/ImportBaggageEntriesSpanProcessor.cs) in every Span that occurs throughout the applications
							tracerProviderBuilder.AddProcessor<ImportBaggageEntriesSpanProcessor>();
						}
					)
					.AddOpenTelemetry()
					.ConfigureResource(res => res
								.AddService(
									context.HostingEnvironment.ApplicationName,
									serviceNamespace: $"LeadManagerMessageConsumer-{context.HostingEnvironment.ApplicationName}",
									serviceVersion: Assembly.GetCallingAssembly().GetName().Version!.ToString()
								)
								.AddAttributes(
									[
										//new KeyValuePair<string,object>("service.version",Assembly.GetExecutingAssembly().GetName().Version!.ToString())
										new KeyValuePair<string,object>("service.env", context.HostingEnvironment.EnvironmentName)
									]
								)
					)
					.WithTracing(trc => trc
						.AddSource(RabbitMqDiagnostics.ActivitySourceName) //Adds RabbitMQ activities to the tracing
						.AddConsoleExporter()
						.AddOtlpExporter(cfg => cfg.Endpoint = OtlpEndpoint)
					);

					_addHostedService.ForEach(addHSvc => addHSvc(services));
				})
				.Build()
				.UseMessageBusInitialization();
}
