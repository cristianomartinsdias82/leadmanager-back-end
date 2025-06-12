using Application.Reporting.Core;
using Dapper;
using LeadManager.BackendServices.ReportGeneration.Core.Configuration;
using Polly;
using Polly.Retry;
using Shared.Settings;
using System.Data.Common;

namespace LeadManager.BackendServices.ReportGeneration.DataService;

internal class ReportGenerationRequestsDataService
{
	private readonly DataSourceSettings _dataSourceSettings;
	private readonly DbProviderFactory _dbProviderFactory;
	private readonly ReportGenerationWorkerSettings _reportGenerationWorkerSettings;
	private readonly TimeProvider _timeProvider;
	private readonly ILogger<ReportGenerationRequestsDataService> _logger;

	public ReportGenerationRequestsDataService(
		DataSourceSettings dataSourceSettings,
		ReportGenerationWorkerSettings reportGenerationWorkerSettings,
		TimeProvider timeProvider,
		ILogger<ReportGenerationRequestsDataService> logger)
	{
		_dataSourceSettings = dataSourceSettings;
		_reportGenerationWorkerSettings = reportGenerationWorkerSettings;
		_timeProvider = timeProvider;
		_logger = logger;

		_dbProviderFactory = DbProviderFactories.GetFactory(dataSourceSettings.ProviderName);
	}

	public async Task<List<ReportGenerationRequest>> ClaimAndGetPendingRequestsAsync(
		CancellationToken cancellationToken)
	{
		using var connection = _dbProviderFactory.CreateConnection()!;
		connection.ConnectionString = _dataSourceSettings.ConnectionString;

		return await ExecuteAsync<List<ReportGenerationRequest>>(
			async (settings, ct) =>
			{
				try
				{
					await connection.OpenAsync(ct);

					var requests = await connection.QueryAsync<ReportGenerationRequest>(
						$"""
						UPDATE TOP({_reportGenerationWorkerSettings.RequestProcessingBatchMaxSize}) Processes.ReportGenerationRequests
						SET Status = 'ReadyForProcessing'
						OUTPUT
							 INSERTED.Id
							,INSERTED.UserId
							,INSERTED.Feature
							,INSERTED.SerializedParameters
							,INSERTED.SerializedParametersDataType
							,INSERTED.Status
							,INSERTED.RequestedAt
							,INSERTED.LastProcessedDate
							,INSERTED.ExecutionAttempts
						FROM Processes.ReportGenerationRequests WITH (ROWLOCK, READPAST, UPDLOCK)
						WHERE Status = 'Pending';
						""");

					return [.. requests];
				}
				finally
				{
					await connection.CloseAsync();

					_logger.LogInformation("{Method} invoked successfully.", nameof(ClaimAndGetPendingRequestsAsync));
				}
			},
			_dataSourceSettings,
			cancellationToken);
	}

	public async Task MarkAsProcessingAsync(
		//int requestId,
		ReportGenerationRequest request,
		DataSourceSettings dataSourceSettings,
		CancellationToken cancellationToken = default)
	{
		using var connection = _dbProviderFactory.CreateConnection()!;
		connection.ConnectionString = dataSourceSettings.ConnectionString;

		await ExecuteAsync(
			async (settings, ct) =>
			{
				try
				{
					await connection.OpenAsync(ct);

					await connection.ExecuteAsync(
						"""
							UPDATE P SET
								 P.Status = 'Processing'
								,P.ExecutionAttempts += 1
								,P.LastProcessedDate = @LastProcessedDate
							FROM Processes.ReportGenerationRequests P (ROWLOCK)
							WHERE
								 P.Id = @Id;
							""", param: new { LastProcessedDate = _timeProvider.GetLocalNow(), request.Id });
				}
				finally
				{
					await connection.CloseAsync();

					_logger.LogInformation("{Method} marked report generation request {Id} as 'Processing'.", nameof(MarkAsProcessingAsync), request.Id);
				}
			},
			dataSourceSettings,
			cancellationToken);
	}

	public async Task<int> MarkAsSucceededAsync(
		ReportGenerationRequest request,
		DataSourceSettings dataSourceSettings,
		string generatedFileFullPath,
		CancellationToken cancellationToken = default)
	{
		using var connection = _dbProviderFactory.CreateConnection()!;
		connection.ConnectionString = dataSourceSettings.ConnectionString;

		return await ExecuteAsync(
			async (settings, ct) =>
			{
				try
				{
					await connection.OpenAsync(ct);

					return await connection.ExecuteAsync(
						"""
						UPDATE P SET
							 P.Status = 'Successful'
							,P.GeneratedFileFullPath = @GeneratedFileFullPath
						FROM Processes.ReportGenerationRequests P (ROWLOCK)
						WHERE P.Id = @Id;
						""", param: new { request.Id, generatedFileFullPath });
				}
				catch (Exception exc)
				{
					_logger.LogError(exc, "Error when executing {Method}. Message: {Message}", nameof(MarkAsFailedAsync), exc.Message);

					return 0;
				}
				finally
				{
					await connection.CloseAsync();

					_logger.LogInformation("{Method} marked report generation request {Id} as 'Successful'.", nameof(MarkAsSucceededAsync), request.Id);
				}
			},
			dataSourceSettings,
			cancellationToken);
	}

	public async Task MarkAsFailedAsync(
		ReportGenerationRequest request,
		DataSourceSettings dataSourceSettings,
		int maxProcessingAttempts,
		Action<int, ReportGenerationRequest>? onAttemptsThresholdReached = default,
		CancellationToken cancellationToken = default)
	{
		using var connection = _dbProviderFactory.CreateConnection()!;
		connection.ConnectionString = dataSourceSettings.ConnectionString;

		await ExecuteAsync(
			async (settings, ct) =>
			{
				var processingAttemptCount = 0;
				var attemptsThresholdReached = false;

				try
				{
					await connection.OpenAsync(ct);

					processingAttemptCount = await connection.ExecuteScalarAsync<int>(
						"""
						SELECT P.ExecutionAttempts
						FROM Processes.ReportGenerationRequests P
						WHERE P.Id = @Id;
						""", new { request.Id });

					attemptsThresholdReached = processingAttemptCount == maxProcessingAttempts;

					await connection.ExecuteAsync(
						"""
						UPDATE P SET P.Status = @Status
						FROM Processes.ReportGenerationRequests P (ROWLOCK)
						WHERE P.Id = @Id;
						""", param: new { Status = processingAttemptCount == maxProcessingAttempts ? "Failed" : "Pending", request.Id });

					if (attemptsThresholdReached && onAttemptsThresholdReached is not null)
						onAttemptsThresholdReached(processingAttemptCount, request);
				}
				catch(Exception exc)
				{
					_logger.LogError(exc, "Error when executing {Method}. Message: {Message}", nameof(MarkAsFailedAsync), exc.Message);
				}
				finally
				{
					await connection.CloseAsync();

					_logger.LogInformation("{Method} marked report generation request {Id} as '{Status}'." +
						" (Attempt number #{AttemptNumber})." +
						" Attempts threshold reached: {ThresholdReached}.",
						nameof(MarkAsSucceededAsync),
						request.Id,
						attemptsThresholdReached ? "Failed" : "Pending",
						processingAttemptCount,
						attemptsThresholdReached ? "Yes" : "No");
				}
			},
			dataSourceSettings,
			cancellationToken);
	}

	private static Task<T> ExecuteAsync<T>(
		Func<DataSourceSettings, CancellationToken, Task<T>> func,
		DataSourceSettings dataSourceSettings,
		CancellationToken cancellationToken)
		=> GetPolicy(dataSourceSettings.RetryOperationMaxCount)
			.ExecuteAsync(async () => await func(dataSourceSettings, cancellationToken));

	private static Task ExecuteAsync(
		Func<DataSourceSettings, CancellationToken, Task> func,
		DataSourceSettings dataSourceSettings,
		CancellationToken cancellationToken)
		=> GetPolicy(dataSourceSettings.RetryOperationMaxCount)
			.ExecuteAsync(async () => await func(dataSourceSettings, cancellationToken));

	private static AsyncRetryPolicy GetPolicy(int maxAttempts)
		=> Policy
			.Handle<DbException>()
			.WaitAndRetryAsync(
				maxAttempts,
				count => TimeSpan.FromSeconds(Math.Pow(2, count) + Random.Shared.Next(2, 4)));
}
