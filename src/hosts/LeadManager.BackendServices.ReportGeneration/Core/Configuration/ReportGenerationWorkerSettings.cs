namespace LeadManager.BackendServices.ReportGeneration.Core.Configuration;

public sealed class ReportGenerationWorkerSettings
{
	public required int ProbingTimeInSecs { get; init; }

	public required LeadsListReportGenerationSettings LeadsListReportGenerationSettings { get; init; }
	public required UsersActionsReportGenerationSettings UsersActionsReportGenerationSettings { get; init; }
}
