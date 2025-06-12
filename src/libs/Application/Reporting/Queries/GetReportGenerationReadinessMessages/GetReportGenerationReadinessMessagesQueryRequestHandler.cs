using Application.Core.Contracts.Persistence;
using CrossCutting.Security.IAM;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.RequestHandling;
using Shared.Results;

namespace Application.Reporting.Queries.GetReportGenerationReadinessMessages;

internal sealed class GetReportGenerationReadinessMessagesQueryRequestHandler
    : ApplicationRequestHandler<GetReportGenerationReadinessMessagesQueryRequest, List<ReportGenerationRequestDto>>
{
    private readonly ILeadManagerDbContext _dbContext;
	private readonly IUserService _userService;

	public GetReportGenerationReadinessMessagesQueryRequestHandler(
        IMediator mediator,
		IUserService userService,
		ILeadManagerDbContext dbContext)
         : base(mediator, default!)
    {
        _dbContext = dbContext;
        _userService = userService;
	}

    public async override Task<ApplicationResponse<List<ReportGenerationRequestDto>>> Handle(
		GetReportGenerationReadinessMessagesQueryRequest request,
        CancellationToken cancellationToken = default)
    {
        var reportGenerationRequests = await _dbContext.ReportGenerationRequests
                                                         .AsNoTracking()
                                                         .Where(it => it.UserId == _userService.GetUserId()!.ToString() &&
                                                                      it.ReadinessUserNotificationDismissed == false)
                                                         .OrderByDescending(it => it.RequestedAt)
                                                         .ThenByDescending(it => it.LastProcessedDate)
                                                         .ToListAsync(cancellationToken);

		return ApplicationResponse<List<ReportGenerationRequestDto>>
                .Create(
                    reportGenerationRequests
                    .Select(ReportGenerationRequestMapper.ToDto)
                    .ToList());
    }
}
