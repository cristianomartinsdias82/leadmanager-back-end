using MediatR;
using Shared.DataQuerying;
using Shared.Results;

namespace Application.Reporting.Commands.RequestReportGeneration;

public sealed record RequestReportGenerationCommandRequest(
	string Format,
	QueryOptions? Query)
	: IRequest<ApplicationResponse<RequestReportGenerationCommandResponse>>;
