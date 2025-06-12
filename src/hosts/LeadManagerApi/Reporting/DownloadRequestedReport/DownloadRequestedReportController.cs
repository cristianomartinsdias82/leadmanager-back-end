using Application.Reporting.Queries.DownloadGeneratedReport;
using CrossCutting.Security.Authorization;
using LeadManagerApi.Core.ApiFeatures;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shared.Results;
using System.Net.Mime;
using static Application.Security.LeadManagerSecurityConfiguration;

namespace LeadManagerApi.Reporting.DownloadRequestedReport;

[ReportingRoute]
[RequiredAllPermissions(Permissions.Read)]
public sealed class DownloadRequestedReportController : LeadManagerController
{
	public DownloadRequestedReportController(ISender mediator) : base(mediator) { }

	[HttpGet("{id}/download")]
	[ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(ApplicationResponse<PersistableData?>),StatusCodes.Status404NotFound)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public async Task<IActionResult> DownloadRequestedReportAsync(
		int id,
		CancellationToken cancellationToken)
	{
		var result = await Mediator.Send(new DownloadGeneratedReportQueryRequest { Id = id }, cancellationToken);

		if (result.Data is null)
			return Result(result);

		return await Task.FromResult(
							File(
								result.Data.DataBytes,
								result.Data.ContentType ?? MediaTypeNames.Application.Octet,
								result.Data.Name));
	}
}