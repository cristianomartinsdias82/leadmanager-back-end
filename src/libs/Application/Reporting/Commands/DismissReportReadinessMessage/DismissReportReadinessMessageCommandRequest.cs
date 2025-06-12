using MediatR;
using Shared.Results;

namespace Application.Reporting.Commands.DismissReportReadinessMessage;

public sealed record DismissReportReadinessMessageCommandRequest(int Id)
	: IRequest<ApplicationResponse<DismissReportReadinessMessageCommandResponse>>;
