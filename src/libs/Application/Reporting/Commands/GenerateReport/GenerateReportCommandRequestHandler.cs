using Application.Core.Contracts.Reporting;
using Application.Reporting.Core;
using MediatR;
using Microsoft.Extensions.Logging;
using Shared.Events.EventDispatching;
using Shared.Exportation;
using Shared.RequestHandling;
using Shared.Results;
using System.Text.Json;

namespace Application.Reporting.Commands.GenerateReport;

internal sealed class GenerateReportCommandRequestHandler : ApplicationRequestHandler<GenerateReportCommandRequest, GenerateReportCommandResponse>
{
	private readonly Func<ReportGenerationFeatures, ExportFormats, IReportGeneration> _reportGeneratorFactory;
	private readonly ILogger<GenerateReportCommandRequestHandler> _logger;

	public GenerateReportCommandRequestHandler(
		Func<ReportGenerationFeatures, ExportFormats, IReportGeneration> reportGeneratorFactory,
		ILogger<GenerateReportCommandRequestHandler> logger,
		IMediator mediator,
		IEventDispatching eventDispatcher)
		: base(mediator, eventDispatcher)
	{
		_logger = logger;
		_reportGeneratorFactory = reportGeneratorFactory;
	}

	public async override Task<ApplicationResponse<GenerateReportCommandResponse>> Handle(
		GenerateReportCommandRequest request,
		CancellationToken cancellationToken = default)
	{
		var reportGenerator = _reportGeneratorFactory(request.Feature, request.ReportGenerationRequestArgs.ExportFormat);

		var result = await reportGenerator.GenerateAsync(request.ReportGenerationRequestArgs.QueryOptions, cancellationToken);

		if (result.Success)
		{
			if (request.OnSuccess is not null)
			{
				try
				{
					await request.OnSuccess(cancellationToken);
				}
				catch { }
			}

			return ApplicationResponse<GenerateReportCommandResponse>
				.Create(new GenerateReportCommandResponse(result.Data));
		}

		_logger.LogError("Error while trying to generate report." +
			" - Message: {Message}." +
			" - Operation code: {OperationCode}" +
			" - Request id: {RequestId}" +
			" - Requested report: {Feature}" +
			" - Format: {ExportFormat}" +
			" - Query options: {QueryOptions}",
			result.Message,
			result.OperationCode,
			request.RequestId,
			request.Feature,
			request.ReportGenerationRequestArgs.ExportFormat,
			JsonSerializer.Serialize(request.ReportGenerationRequestArgs.QueryOptions));

		if (request.OnFailure is not null)
		{
			try
			{
				await request.OnFailure(cancellationToken);
			}
			catch { }
		}

		return ApplicationResponse<GenerateReportCommandResponse>
				.Create(
					new GenerateReportCommandResponse(result.Data),
					message: result.Message,
					operationCode: result.OperationCode,
					exception: result.Exception,
					inconsistencies: [..result.Inconsistencies ?? []]);
	}
}
