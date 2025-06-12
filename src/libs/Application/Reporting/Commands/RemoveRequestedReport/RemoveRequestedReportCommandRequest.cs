using MediatR;
using Shared.Results;

namespace Application.Reporting.Commands.RemoveRequestedReport;

public sealed record RemoveRequestedReportCommandRequest(int Id)
	: IRequest<ApplicationResponse<RemoveRequestedReportCommandResponse>>;
