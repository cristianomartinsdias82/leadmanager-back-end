using Application.Features.Leads.Queries.SearchLead;
using CrossCutting.Security.Authorization;
using LeadManagerApi.Core.ApiFeatures;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shared.Results;
using static LeadManagerApi.Core.Configuration.Security.LeadManagerApiSecurityConfiguration;

namespace LeadManagerApi.Leads.SearchLead;

[LeadsRoute]
//[Authorize(Policy = LeadManagerApiSecurityConfiguration.Policies.LeadManagerDefaultPolicy)]
[RequiredAllPermissions(Permissions.Read)]
public sealed class SearchLeadController : LeadManagerController
{
    public SearchLeadController(ISender mediator) : base(mediator) { }

    [HttpGet("search")]
    [ProducesResponseType(typeof(ApplicationResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApplicationResponse<bool>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApplicationResponse<bool>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApplicationResponse<bool>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApplicationResponse<bool>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> SearchLead(SearchLeadQueryRequest request, CancellationToken cancellationToken)
    {
        var response = await Mediator.Send(request, cancellationToken);

        return Result(response);
    }
}