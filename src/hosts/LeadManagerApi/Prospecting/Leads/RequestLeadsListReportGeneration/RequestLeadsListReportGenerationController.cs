using Application.Reporting.Commands.RequestReportGeneration;
using CrossCutting.Security.Authorization;
using LeadManagerApi.Core.ApiFeatures;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shared.DataQuerying;
using static Application.Security.LeadManagerSecurityConfiguration;

namespace LeadManagerApi.Prospecting.Leads.GetUploadedLeadsFiles;

[LeadsRoute("reporting/leads-list")]
[RequiredAllPermissions(Permissions.Read)]
public sealed class RequestLeadsListReportGenerationController : LeadManagerController
{
	public RequestLeadsListReportGenerationController(ISender mediator) : base(mediator) { }

	[HttpPost]
	[ProducesResponseType(StatusCodes.Status202Accepted)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public async Task<IActionResult> RequestLeadsListReportGenerationAsync(
		[FromQuery] string format,
		[FromBody] QueryOptions? queryOptions,
		CancellationToken cancellationToken)
    {
        var response = await Mediator.Send(new RequestReportGenerationCommandRequest(format, queryOptions), cancellationToken);

		return Result(
				response,
				onSuccessStatusCodeFactory: (_, _) => StatusCodes.Status202Accepted);
    }
}