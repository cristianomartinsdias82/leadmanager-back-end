using Shared.DataQuerying;
using Shared.Exportation;

namespace Application.Reporting.Core;

public sealed class ReportGenerationRequestArgs
{
	public required ExportFormats ExportFormat { get; init; }
	public QueryOptions? QueryOptions { get; init; } = default!;
}