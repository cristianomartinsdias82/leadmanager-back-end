namespace LeadManager.BackendServices.ReportGeneration.Core.Configuration;

internal sealed class ReportGenerationWorkerSettings
{
	public required int ProbingTimeInSecs { get; init; }
	public required int MaxProcessingAttempts { get; init; }
	public required int RequestProcessingBatchMaxSize { get; init; }

	public required LeadsListReportGenerationSettings LeadsListReportGenerationSettings { get; init; }
	public required UsersActionsReportGenerationSettings UsersActionsReportGenerationSettings { get; init; }
}
