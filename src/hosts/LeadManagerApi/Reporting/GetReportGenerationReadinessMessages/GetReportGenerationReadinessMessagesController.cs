using Application.Reporting.Queries.GetReportGenerationReadinessMessages;
using CrossCutting.Security.Authorization;
using LeadManagerApi.Core.ApiFeatures;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shared.Results;
using static Application.Security.LeadManagerSecurityConfiguration;

namespace LeadManagerApi.Reporting.GetReportGenerationReadinessMessages;

[ReportingRoute]
[RequiredAllPermissions(Permissions.Read)]
public sealed class GetReportGenerationReadinessMessagesController : LeadManagerController
{
	public GetReportGenerationReadinessMessagesController(ISender mediator) : base(mediator) { }

	[HttpGet("readiness-messages")]
	[ProducesResponseType(typeof(ApplicationResponse<List<ReportGenerationRequestDto>>), StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public async Task<IActionResult> GetReportGenerationReadinessMessagesAsync(CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetReportGenerationReadinessMessagesQueryRequest(), cancellationToken);

		return Result(result);
    }
}