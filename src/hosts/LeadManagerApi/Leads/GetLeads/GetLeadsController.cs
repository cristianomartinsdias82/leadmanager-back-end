using Application.Features.Leads.Queries.GetLeads;
using Application.Features.Leads.Shared;
using CrossCutting.Security.Authorization;
using LeadManagerApi.Core.ApiFeatures;
using LeadManagerApi.Core.Configuration.Security;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shared.DataPagination;

namespace LeadManagerApi.Leads.GetLeads;

[LeadsRoute]
//[Authorize(Policy = LeadManagerApiSecurityConfiguration.Policies.LeadManagerDefaultPolicy)]
[RequiredAllPermissions(requiredPermissions: LeadManagerApiSecurityConfiguration.Claims.Read)]
public sealed class GetLeadsController : LeadManagerController
{
    public GetLeadsController(ISender sender) : base(sender) { }

    [HttpGet]
    [ProducesResponseType(typeof(PagedList<LeadDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetLeadsAsync([FromQuery]PaginationOptions paginationOptions, CancellationToken cancellationToken)
    {
        var response = await Mediator.Send(new GetLeadsQueryRequest(paginationOptions), cancellationToken);

        return Result(response);
    }
}