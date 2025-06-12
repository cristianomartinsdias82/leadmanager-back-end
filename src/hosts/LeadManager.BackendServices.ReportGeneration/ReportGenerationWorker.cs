using Application.Reporting.Commands.GenerateReport;
using Application.Reporting.Core;
using LeadManager.BackendServices.ReportGeneration.Core.Configuration;
using LeadManager.BackendServices.ReportGeneration.DataService;
using MediatR;
using Shared.Settings;
using System.Text.Json;

namespace LeadManager.BackendServices.ReportGeneration;

internal class ReportGenerationWorker : BackgroundService
{
	private readonly ILogger<ReportGenerationWorker> _logger;
	private readonly ReportGenerationRequestsDataService _dataService;
	private readonly ReportGenerationWorkerSettings _reportGenerationWorkerSettings;
	private readonly DataSourceSettings _dataSourceSettings;
	private readonly TimeProvider _timeProvider;
	private readonly IServiceProvider _serviceProvider;

	public ReportGenerationWorker(
		ReportGenerationRequestsDataService dataService,
		ReportGenerationWorkerSettings reportGenerationWorkerSettings,
		DataSourceSettings dataSourceSettings,
		IServiceProvider serviceProvider,
		TimeProvider timeProvider,
		ILogger<ReportGenerationWorker> logger)
	{
		_dataService = dataService;
		_reportGenerationWorkerSettings = reportGenerationWorkerSettings;
		_dataSourceSettings = dataSourceSettings;
		_serviceProvider = serviceProvider;
		_timeProvider = timeProvider;
		_logger = logger;
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		while (!stoppingToken.IsCancellationRequested)
		{
			if (_logger.IsEnabled(LogLevel.Information))
				_logger.LogInformation("Worker running at: {time}", _timeProvider.GetLocalNow());

			var pendingRequests = await _dataService
											.ClaimAndGetPendingRequestsAsync(stoppingToken);

			var reportGenerationRequestsData = pendingRequests
												.Select(req => (Request: req,
																Args: JsonSerializer.Deserialize<ReportGenerationRequestArgs>(req.SerializedParameters!)));

			await Parallel.ForEachAsync(reportGenerationRequestsData,
										stoppingToken,
										async (data, ct) =>
										{
											using var scope = _serviceProvider.CreateScope();
											var sender = scope.ServiceProvider.GetRequiredService<IMediator>();

											await _dataService.MarkAsProcessingAsync(
													data.Request,
													_dataSourceSettings,
													stoppingToken);

											await sender.Send(new GenerateReportCommandRequest(
												data.Request.UserId,
												data.Request.Id,
												data.Args!,
												data.Request.Feature,
												async(generatedFileFullPath, stoppingToken) => await _dataService.MarkAsSucceededAsync(
																								data.Request,
																								_dataSourceSettings,
																								generatedFileFullPath,
																								stoppingToken),
												async(stoppingToken) => await _dataService.MarkAsFailedAsync(
																								data.Request,
																								_dataSourceSettings,
																								_reportGenerationWorkerSettings.MaxProcessingAttempts,
																								OnReportGenerationAttemptsThresholdReached,
																								stoppingToken)
											), stoppingToken);
										});

			//Wait for...
			await Task.Delay(_reportGenerationWorkerSettings.ProbingTimeInSecs * 1000, stoppingToken);
		}
	}

	private void OnReportGenerationAttemptsThresholdReached(
		int maxProcessingAttempts,
		ReportGenerationRequest request)
	{
		//TODO: Replace this logic by one that either sends a message to a queue, or texts/emails the user
		//reporting him/her there was a problem when trying to generate the requested report.
		_logger.LogError("Dear {user}, a problem occurred during the report generation process and the development" +
			" team is looking into it as soon as possible. We apologize the inconvenient.", request.UserId);
	}
}
