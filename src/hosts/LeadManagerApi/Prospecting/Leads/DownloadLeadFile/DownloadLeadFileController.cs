using Application.Prospecting.Leads.Queries.DownloadLeadsFile;
using LeadManagerApi.Core.ApiFeatures;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using static Application.Security.LeadManagerSecurityConfiguration;

namespace LeadManagerApi.Prospecting.Leads.DownloadLeadFile;

[LeadsRoute("uploaded-files")]
[Authorize(Policy = Policies.LeadManagerAdministrativeTasksPolicy)]
public sealed class DownloadLeadFileController : LeadManagerController
{
	public DownloadLeadFileController(ISender mediator) : base(mediator) { }

	[HttpGet("{id:Guid}/download")]
	[ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public async Task<IActionResult> DownloadFileAsync(
		Guid id,
		CancellationToken cancellationToken)
	{
		var result = await Mediator.Send(new DownloadLeadsFileQueryRequest { Id = id }, cancellationToken);
		if (result.Data is null)
			return Result(result);

		return await Task.FromResult(
							File(
								result.Data.FileBytes,
								result.Data.ContentType ?? MediaTypeNames.Application.Octet,
								result.Data.FileName));
	}
}