namespace Application.Reporting.Queries.GetReportGenerationReadinessMessages;

public sealed record ReportGenerationRequestDto(
	int Id,
	DateTimeOffset RequestedAt,
	string Status,
	string Feature
);