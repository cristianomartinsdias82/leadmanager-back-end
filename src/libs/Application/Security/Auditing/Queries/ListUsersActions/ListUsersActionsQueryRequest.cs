using MediatR;
using Shared.DataQuerying;
using Shared.Results;

namespace Application.Security.Auditing.Queries.ListUsersActions;

public sealed record ListUsersActionsQueryRequest(
	PaginationOptions PaginationOptions,
	QueryOptions QueryOptions)
	: IRequest<ApplicationResponse<PagedList<AuditEntryDto>>>;