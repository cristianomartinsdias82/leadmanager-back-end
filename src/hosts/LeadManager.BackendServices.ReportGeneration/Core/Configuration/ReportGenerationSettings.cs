namespace LeadManager.BackendServices.ReportGeneration.Core.Configuration;

internal abstract class ReportGenerationSettings
{
	public string DropRootPath { get; init; } = default!;
}
