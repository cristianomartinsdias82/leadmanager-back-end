using MediatR;
using Shared.Results;

namespace Application.Reporting.Queries.GetReportGenerationReadinessMessages;

public sealed record GetReportGenerationReadinessMessagesQueryRequest()
						: IRequest<ApplicationResponse<List<ReportGenerationRequestDto>>>;