using Application.Reporting.Commands.RemoveRequestedReport;
using CrossCutting.Security.Authorization;
using LeadManagerApi.Core.ApiFeatures;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shared.Results;
using static Application.Security.LeadManagerSecurityConfiguration;

namespace LeadManagerApi.Reporting.RemoveRequestedReport;

[ReportingRoute]
[RequiredAllPermissions(Permissions.Read)]
public sealed class RemoveRequestedReport : LeadManagerController
{
	public RemoveRequestedReport(ISender mediator) : base(mediator) { }

	[HttpDelete("{id}")]
	[ProducesResponseType(typeof(ApplicationResponse<RemoveRequestedReportCommandResponse>), StatusCodes.Status204NoContent)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public async Task<IActionResult> RemoveRequestedReportAsync(
		int id,
		CancellationToken cancellationToken)
	{
		var result = await Mediator.Send(new RemoveRequestedReportCommandRequest(id), cancellationToken);

		return Result(
				result,
				onSuccessStatusCodeFactory: (_, _) => StatusCodes.Status204NoContent);
	}
}