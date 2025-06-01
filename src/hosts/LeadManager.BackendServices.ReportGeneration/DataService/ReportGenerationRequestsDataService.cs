using Domain.Reporting;
using LeadManager.BackendServices.ReportGeneration.Core.Configuration;
using Polly;
using Polly.Retry;
using System.Data.Common;
using Dapper;

namespace LeadManager.BackendServices.ReportGeneration.DataService;

internal static class ReportGenerationRequestsDataService
{
	private static DbProviderFactory? _dbProviderFactory;

	public static async Task<List<ReportGenerationRequest>> ClaimAndGetPendingRequestsAsync(
		DataSourceSettings dataSourceSettings,
		CancellationToken cancellationToken)
	{
		_dbProviderFactory ??= DbProviderFactories.GetFactory(dataSourceSettings.ProviderName);

		using var connection = _dbProviderFactory.CreateConnection()!;
		connection.ConnectionString = dataSourceSettings.ConnectionString;

		return await ExecuteAsync<List<ReportGenerationRequest>>(
			async (settings, ct) => {
				try
				{
					await connection.OpenAsync(ct);

					var requests = await connection.QueryAsync<ReportGenerationRequest>(
						"""
						UPDATE Processes.ReportGenerationRequests
						SET
							 Status = 'Processing'
							,ExecutionAttempts = ExecutionAttempts + 1
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
						WHERE
								Status = 'Pending'
						""");

					return [..requests];
				}
				finally
				{
					await connection.CloseAsync();
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

	private static AsyncRetryPolicy GetPolicy(int maxAttempts)
		=> Policy
			.Handle<DbException>()
			.WaitAndRetryAsync(
				maxAttempts,
				count => TimeSpan.FromSeconds(Math.Pow(2, count) + Random.Shared.Next(2, 4)));
}
