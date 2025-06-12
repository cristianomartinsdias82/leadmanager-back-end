using Application.Core.Contracts.Persistence;
using Application.Reporting.Core;
using CrossCutting.Security.IAM;
using MediatR;
using Shared.Events.EventDispatching;
using Shared.Exportation;
using Shared.RequestHandling;
using Shared.Results;
using System.Text.Json;

namespace Application.Reporting.Commands.RequestReportGeneration;

internal sealed class RequestReportGenerationCommandRequestHandler : ApplicationRequestHandler<RequestReportGenerationCommandRequest, RequestReportGenerationCommandResponse>
{
	private readonly IUserService _userService;
	private readonly ILeadManagerDbContext _dbContext;
	private readonly TimeProvider _timeProvider;

	public RequestReportGenerationCommandRequestHandler(
		IMediator mediator,
		IEventDispatching eventDispatcher,
		IUserService userService,
		ILeadManagerDbContext dbContext,
		TimeProvider timeProvider) : base(mediator, eventDispatcher)
	{
		_userService = userService;
		_dbContext = dbContext;
		_timeProvider = timeProvider;
	}

	public async override Task<ApplicationResponse<RequestReportGenerationCommandResponse>> Handle(
		RequestReportGenerationCommandRequest request,
		CancellationToken cancellationToken = default)
	{
		var reportGenerationRequest = ReportGenerationRequest
										.Create(
											ReportGenerationFeatures.LeadsList,
											JsonSerializer.Serialize(new ReportGenerationRequestArgs
											{
												ExportFormat = Enum.Parse<ExportFormats>(request.Format, true),
												QueryOptions = request.Query
											}),
											$"{typeof(ReportGenerationRequestArgs).AssemblyQualifiedName}",
											_timeProvider.GetLocalNow(),
											_userService.GetUserId().ToString()!);

		_dbContext.ReportGenerationRequests.Add(reportGenerationRequest);
		await _dbContext.SaveChangesAsync(cancellationToken);

		return ApplicationResponse<RequestReportGenerationCommandResponse>
				.Create(new RequestReportGenerationCommandResponse(reportGenerationRequest.Id));
	}
}
