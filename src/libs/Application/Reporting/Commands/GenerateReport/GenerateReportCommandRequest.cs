using Application.Reporting.Core;
using MediatR;
using Shared.Results;

namespace Application.Reporting.Commands.GenerateReport;

public sealed record GenerateReportCommandRequest(
	string ReceiverId,
	int RequestId,
	ReportGenerationRequestArgs ReportGenerationRequestArgs,
	ReportGenerationFeatures Feature,
	Func<string, CancellationToken, Task>? OnSuccess = default,
	Func<CancellationToken, Task>? OnFailure = default)
	: IRequest<ApplicationResponse<GenerateReportCommandResponse>>;
