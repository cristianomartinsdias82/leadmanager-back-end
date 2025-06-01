using LeadManager.BackendServices.ReportGeneration.Core.Configuration;
using LeadManager.BackendServices.ReportGeneration.DataService;
using Shared.Reporting;
using System.Text.Json;

namespace LeadManager.BackendServices.ReportGeneration;

public class ReportGenerationWorker : BackgroundService
{
	private readonly ILogger<ReportGenerationWorker> _logger;
	private readonly ReportGenerationWorkerSettings _reportGenerationWorkerSettings;
	private readonly DataSourceSettings _dataSourceSettings;
	private readonly TimeProvider _timeProvider;
	private readonly IServiceProvider _serviceProvider;

	public ReportGenerationWorker(
		ILogger<ReportGenerationWorker> logger,
		ReportGenerationWorkerSettings reportGenerationWorkerSettings,
		DataSourceSettings dataSourceSettings,
		TimeProvider timeProvider,
		IServiceProvider serviceProvider)
	{
		_logger = logger;
		_reportGenerationWorkerSettings = reportGenerationWorkerSettings;
		_dataSourceSettings = dataSourceSettings;
		_timeProvider = timeProvider;
		_serviceProvider = serviceProvider;
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		while (!stoppingToken.IsCancellationRequested)
		{
			if (_logger.IsEnabled(LogLevel.Information))
				_logger.LogInformation("Worker running at: {time}", _timeProvider.GetLocalNow());

			var pendingRequests = await ReportGenerationRequestsDataService
											.ClaimAndGetPendingRequestsAsync(
												_dataSourceSettings,
												stoppingToken);

			var reportGenerationRequestsData = pendingRequests
												.Select(req => (Request: req,
																Args: JsonSerializer.Deserialize<ReportGenerationRequestArgs>(req.SerializedParameters!)));

			await Parallel.ForEachAsync(reportGenerationRequestsData,
										stoppingToken,
										async (data, ct) =>
										{
											using var scope = _serviceProvider.CreateScope();
											//var reportGenerator = scope.ServiceProvider.GetKeyedService<IReportGeneration>(ReportGenerationFormats.Pdf);
											//await reportGenerator
											//	.GenerateAsync(
											//		receiver: /*data.Request.UserId*/,
											//		queryOptions: /*data.Args.QueryOptions*/,
											//		onSuccess(ct) => ReportGenerationRequestsDataService.MarkAsSucceeded(data.Request, ct)),
											//		onFailure(ct) => ReportGenerationRequestsDataService.MarkAsFailed(data.Request, ct)),
											//		ct);

											await Task.Delay(0, ct);
										});

			//Wait for...
			await Task.Delay(_reportGenerationWorkerSettings.ProbingTimeInSecs * 1000, stoppingToken);
		}
	}
}
