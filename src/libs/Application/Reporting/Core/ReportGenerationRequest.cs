using Shared.Results;

namespace Application.Reporting.Core;

public sealed class ReportGenerationRequest
{
	private ReportGenerationRequest() { }

	public int Id { get; private set; }
	public string UserId { get; private set; } = default!;
	public DateTimeOffset RequestedAt { get; private set; }
	public DateTimeOffset? LastProcessedDate { get; set; }
	public ReportGenerationFeatures Feature { get; private set; }
	public string? SerializedParameters { get; private set; } = default!;
	public string? SerializedParametersDataType { get; private set; }
	public int ExecutionAttempts { get; private set; }
	public OperationStatuses Status { get; private set; }
	public bool ReadinessUserNotificationDismissed { get; private set; }
	public string? GeneratedFileName { get; private set; }

	public static ReportGenerationRequest Create(
		ReportGenerationFeatures feature,
		string? serializedParameters,
		string? serializedParametersDataType,
		DateTimeOffset requestedAt,
		string userId)
	{
		if (!string.IsNullOrWhiteSpace(serializedParameters) && string.IsNullOrWhiteSpace(serializedParametersDataType))
			throw new ArgumentException("The serializedParametersDataType argument must be informed when serializedParameters argument is informed.");

		return new()
		{
			RequestedAt = requestedAt,
			Feature = feature,
			Status = OperationStatuses.Pending,
			SerializedParameters = serializedParameters,
			SerializedParametersDataType = serializedParametersDataType,
			UserId = userId
		};
	}

	public void IncrementExecutionAttempts()
		=> ExecutionAttempts += 1;

	public void SetLastProcessedDate(DateTimeOffset dateTimeOffset)
		=> LastProcessedDate = dateTimeOffset;

	public void SetStats(OperationStatuses status)
		=> Status = status;

	public void DismissReadinessNotification()
		=> ReadinessUserNotificationDismissed = true;

	public void SetGeneratedFileName(string fileName)
		=> GeneratedFileName = fileName;
}
