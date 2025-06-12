using Application.Prospecting.Leads.Commands.BulkRemoveLeadsFiles;
using LeadManagerApi.Core.ApiFeatures;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Results;
using static Application.Security.LeadManagerSecurityConfiguration;

namespace LeadManagerApi.Prospecting.Leads.BulkRemoveLeadsFiles;

[LeadsRoute("uploaded-files")]
[Authorize(Policy = Policies.LeadManagerAdministrativeTasksPolicy)]
public sealed class BulkRemoveLeadsFilesController : LeadManagerController
{
	public BulkRemoveLeadsFilesController(ISender mediator) : base(mediator) { }

	[HttpDelete]
	[ProducesResponseType(typeof(ApplicationResponse<bool>), StatusCodes.Status204NoContent)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public async Task<IActionResult> BulkRemoveLeadsFilesAsync(
		[FromBody] BulkRemoveLeadsFilesCommandRequest request,
		CancellationToken cancellationToken)
	{
		var result = await Mediator.Send(request, cancellationToken);

		return Result(
				result,
				onSuccessStatusCodeFactory: (_, _) => StatusCodes.Status204NoContent);
	}
}