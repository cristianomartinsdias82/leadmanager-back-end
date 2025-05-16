using Application.Security.Auditing.Queries.ListUsersActions;
using LeadManagerApi.Core.ApiFeatures;
using LeadManagerApi.Security.Auditing.Core;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.DataQuerying;
using Shared.Results;
using static Application.Security.LeadManagerSecurityConfiguration;

namespace LeadManagerApi.Security.Auditing.ListUsersActions;

[AuditingRoute]
[Authorize(Policy = Policies.LeadManagerAdministrativeTasksPolicy)]
public sealed class ListUsersActionsController : LeadManagerController
{
	public ListUsersActionsController(ISender mediator) : base(mediator) { }

	[HttpGet]
	[ProducesResponseType(typeof(ApplicationResponse<PagedList<AuditEntryDto>>), StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public async Task<IActionResult> ListUsersActionsAsync(
		[FromQuery] PaginationOptions paginationOptions,
		[FromQuery] QueryOptions queryOptions,
		CancellationToken cancellationToken)
    {
        var response = await Mediator.Send(new ListUsersActionsQueryRequest(
												paginationOptions,
												queryOptions),
											cancellationToken);

		return Result(response);
    }
}