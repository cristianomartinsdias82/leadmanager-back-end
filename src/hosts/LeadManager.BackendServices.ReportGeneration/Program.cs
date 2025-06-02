using Application.Core.Configuration;
using Infrastructure.Configuration;
using LeadManager.BackendServices.ReportGeneration.Core.Configuration;
using LeadManager.BackendServices.ReportGeneration.DataService;

namespace LeadManager.BackendServices.ReportGeneration;

internal class Program
{
	public static void Main(string[] args)
	{
		var builder = Host.CreateApplicationBuilder(args);

		builder.Services.AddHostedService<ReportGenerationWorker>();
		builder.Services.AddSingleton(TimeProvider.System);
		var reportGenerationWorkerSettings = builder
												.Configuration
												.GetSection(nameof(ReportGenerationWorkerSettings))
												.Get<ReportGenerationWorkerSettings>()!;
		builder.Services.AddSingleton(reportGenerationWorkerSettings);
		builder.Services.AddSingleton<ReportGenerationRequestsDataService>();

		builder.Services.AddApplicationServices(builder.Configuration, builder.Environment);
		builder.Services.AddInfrastructureServices(builder.Configuration);

		var host = builder.Build();
		host.Run();
	}
}