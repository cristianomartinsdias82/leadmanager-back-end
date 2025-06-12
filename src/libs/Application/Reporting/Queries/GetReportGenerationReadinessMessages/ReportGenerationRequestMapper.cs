using Application.Reporting.Core;

namespace Application.Reporting.Queries.GetReportGenerationReadinessMessages;

internal static class ReportGenerationRequestMapper
{
	public static ReportGenerationRequestDto ToDto(this ReportGenerationRequest reportGenerationRequest)
		=> new(
			reportGenerationRequest.Id,
			reportGenerationRequest.RequestedAt,
			reportGenerationRequest.Status.ToString(),
			reportGenerationRequest.Feature.ToString());
}