using Application.Core.Contracts.Persistence;
using MediatR;
using Shared.Events.EventDispatching;
using Shared.RequestHandling;
using Shared.Results;

namespace Application.Reporting.Commands.DismissReportReadinessMessage;

internal sealed class DismissReportReadinessMessageCommandRequestHandler
	: ApplicationRequestHandler<DismissReportReadinessMessageCommandRequest, DismissReportReadinessMessageCommandResponse>
{
	private readonly ILeadManagerDbContext _dbContext;

	public DismissReportReadinessMessageCommandRequestHandler(
		IMediator mediator,
		IEventDispatching eventDispatcher,
		ILeadManagerDbContext dbContext) : base(mediator, eventDispatcher)
	{
		_dbContext = dbContext;
	}

	public async override Task<ApplicationResponse<DismissReportReadinessMessageCommandResponse>> Handle(
		DismissReportReadinessMessageCommandRequest request,
		CancellationToken cancellationToken = default)
	{
		var reportGenerationRequest = await _dbContext.ReportGenerationRequests.FindAsync(request.Id, cancellationToken);

		if (reportGenerationRequest is null)
			return ApplicationResponse<DismissReportReadinessMessageCommandResponse>
				.Create(
					default!,
					operationCode: OperationCodes.NotFound
				);

		reportGenerationRequest.DismissReadinessNotification();

		await _dbContext.SaveChangesAsync(cancellationToken);

		return ApplicationResponse<DismissReportReadinessMessageCommandResponse>
				.Create(new DismissReportReadinessMessageCommandResponse());
	}
}
