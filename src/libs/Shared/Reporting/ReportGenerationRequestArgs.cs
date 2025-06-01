using Shared.DataQuerying;

namespace Shared.Reporting;

public sealed class ReportGenerationRequestArgs
{
	public required ReportGenerationFormats ExportFormat { get; init; }
	public QueryOptions? QueryOptions { get; init; } = default!;
}