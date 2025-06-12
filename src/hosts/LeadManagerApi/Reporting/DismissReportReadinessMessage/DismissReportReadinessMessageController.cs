using Application.Reporting.Commands.DismissReportReadinessMessage;
using CrossCutting.Security.Authorization;
using LeadManagerApi.Core.ApiFeatures;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shared.Results;
using static Application.Security.LeadManagerSecurityConfiguration;

namespace LeadManagerApi.Reporting.DismissReportReadinessMessage;

[ReportingRoute]
[RequiredAllPermissions(Permissions.Read)]
public sealed class DismissReportReadinessMessageController : LeadManagerController
{
	public DismissReportReadinessMessageController(ISender mediator) : base(mediator) { }

	[HttpPut("readiness-messages/{id}/dismiss")]
	[ProducesResponseType(typeof(ApplicationResponse<DismissReportReadinessMessageCommandResponse>), StatusCodes.Status204NoContent)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public async Task<IActionResult> DismissReportReadinessMessageAsync(int id, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new DismissReportReadinessMessageCommandRequest(id), cancellationToken);

		return Result(
				result,
				onSuccessStatusCodeFactory: (_, _) => StatusCodes.Status204NoContent);
    }
}