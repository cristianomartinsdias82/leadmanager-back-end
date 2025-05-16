using Application.Core.Contracts.Repository.Security.Auditing;
using MediatR;
using Shared.DataQuerying;
using Shared.RequestHandling;
using Shared.Results;

namespace Application.Security.Auditing.Queries.ListUsersActions;

internal sealed class ListUsersActionsQueryRequestHandler : ApplicationRequestHandler<ListUsersActionsQueryRequest, PagedList<AuditEntryDto>>
{
    private readonly IAuditingRepository _auditingRepository;

	public ListUsersActionsQueryRequestHandler(
		IAuditingRepository auditingRepository,
		IMediator mediator)
         : base(mediator, default!)
    {
        _auditingRepository = auditingRepository;
	}

    public async override Task<ApplicationResponse<PagedList<AuditEntryDto>>> Handle(
        ListUsersActionsQueryRequest request,
        CancellationToken cancellationToken = default)
    {
        var pagedAuditEntries = await _auditingRepository.GetAsync(
            request.PaginationOptions,
            request.QueryOptions,
            cancellationToken);

        return ApplicationResponse<PagedList<AuditEntryDto>>
                .Create(pagedAuditEntries.MapPage(ListUsersActionsMapper.Map));
    }
}
