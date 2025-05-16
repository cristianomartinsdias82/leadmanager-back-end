using Application.Security.UsersManagement.Queries.ListUsers;
using LeadManagerApi.Core.ApiFeatures;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.DataQuerying;
using Shared.Results;
using static Application.Security.LeadManagerSecurityConfiguration;

namespace LeadManagerApi.Security.UsersManagement.ListUsers;

[Route("api/users")]
[Authorize(Policy = Policies.LeadManagerAdministrativeTasksPolicy)]
public sealed class ListUsersController : LeadManagerController
{
	public ListUsersController(ISender mediator) : base(mediator) { }

	[HttpGet]
	[ProducesResponseType(typeof(ApplicationResponse<PagedList<UserDto>>), StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public async Task<IActionResult> ListUsersAsync(CancellationToken cancellationToken)
    {
        var response = await Mediator.Send(new ListUsersQueryRequest(), cancellationToken);

		return Result(response);
    }
}