using LeadManager.BackendServices.ReportGeneration.Core.Configuration;
using System.Data.Common;

namespace LeadManager.BackendServices.ReportGeneration;

public class Program
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

		var dataSourceSettings = builder
									.Configuration
									.GetSection(nameof(DataSourceSettings))
									.Get<DataSourceSettings>()!;
		builder.Services.AddSingleton(dataSourceSettings);
		DbProviderFactories.RegisterFactory(dataSourceSettings.ProviderName, Microsoft.Data.SqlClient.SqlClientFactory.Instance);

		var host = builder.Build();
		host.Run();
	}
}