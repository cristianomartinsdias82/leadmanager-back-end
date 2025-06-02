using Application.Core.Contracts.Reporting;
using Application.Reporting.Core;
using CrossCutting.FileStorage;
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
	private readonly IServiceProvider _serviceProvider;
	private readonly Func<IServiceProvider, ReportGenerationFeatures, ExportFormats, IReportGeneration> _reportGeneratorFactory;
	private readonly ILogger<GenerateReportCommandRequestHandler> _logger;
	private readonly IFileStorageProvider _fileStorageProvider;
	private readonly TimeProvider _timeProvider;

	public GenerateReportCommandRequestHandler(
		IServiceProvider serviceProvider,
		Func<IServiceProvider, ReportGenerationFeatures, ExportFormats, IReportGeneration> reportGeneratorFactory,
		ILogger<GenerateReportCommandRequestHandler> logger,
		IMediator mediator,
		IEventDispatching eventDispatcher,
		IFileStorageProvider fileStorageProvider,
		TimeProvider timeProvider)
		: base(mediator, eventDispatcher)
	{
		_logger = logger;
		_serviceProvider = serviceProvider;
		_reportGeneratorFactory = reportGeneratorFactory;
		_fileStorageProvider = fileStorageProvider;
		_timeProvider = timeProvider;
	}

	public async override Task<ApplicationResponse<GenerateReportCommandResponse>> Handle(
		GenerateReportCommandRequest request,
		CancellationToken cancellationToken = default)
	{
		var reportGenerator = _reportGeneratorFactory(
								_serviceProvider,
								request.Feature,
								request.ReportGenerationRequestArgs.ExportFormat);

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

			await _fileStorageProvider.UploadAsync(
										result.Data.DataBytes,
										result.Data.Name ?? $"GeneratedFile-{_timeProvider.GetLocalNow():yyyyMMdd_hhmmss}.dat",
										cancellationToken: cancellationToken);

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
