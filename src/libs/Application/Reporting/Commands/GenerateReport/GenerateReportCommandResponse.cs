using Shared.Results;

namespace Application.Reporting.Commands.GenerateReport;

public sealed record GenerateReportCommandResponse(PersistableData GeneratedReportData);