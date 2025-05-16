using Application.Prospecting.Leads.Queries.GetUploadedLeadsFiles;
using LeadManagerApi.Core.ApiFeatures;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.DataQuerying;
using Shared.Results;
using static Application.Security.LeadManagerSecurityConfiguration;

namespace LeadManagerApi.Prospecting.Leads.GetUploadedLeadsFiles;

[LeadsRoute("uploaded-files")]
[Authorize(Policy = Policies.LeadManagerAdministrativeTasksPolicy)]
public sealed class GetUploadedLeadsFilesController : LeadManagerController
{
	public GetUploadedLeadsFilesController(ISender mediator) : base(mediator) { }

	[HttpGet]
	[ProducesResponseType(typeof(ApplicationResponse<PagedList<UploadedLeadsFileDto>>), StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public async Task<IActionResult> GetUploadedFilesAsync(
		[FromQuery] PaginationOptions paginationOptions,
		CancellationToken cancellationToken)
    {
        var response = await Mediator.Send(new GetUploadedLeadsFilesQueryRequest(paginationOptions), cancellationToken);

		return Result(response);
    }
}