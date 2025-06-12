using Application.Reporting.Commands.RequestReportGeneration;
using CrossCutting.Security.Authorization;
using LeadManagerApi.Core.ApiFeatures;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shared.DataQuerying;
using Shared.Results;
using static Application.Security.LeadManagerSecurityConfiguration;

namespace LeadManagerApi.Reporting.RequestLeadsListReportGeneration;

[ReportingRoute]
[RequiredAllPermissions(Permissions.Read)]
public sealed class RequestLeadsListReportGenerationController : LeadManagerController
{
	public RequestLeadsListReportGenerationController(ISender mediator) : base(mediator) { }

	[HttpPost("leads-list")]
	[ProducesResponseType(typeof(ApplicationResponse<RequestReportGenerationCommandResponse>), StatusCodes.Status202Accepted)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public async Task<IActionResult> RequestLeadsListReportGenerationAsync(
		[FromQuery] string format,
		[FromBody] QueryOptions? queryOptions,
		CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new RequestReportGenerationCommandRequest(format, queryOptions), cancellationToken);

		return Result(
				result,
				onSuccessStatusCodeFactory: (_, _) => StatusCodes.Status202Accepted);
    }
}