using Application.Prospecting.Leads.Queries.GetLeads;
using CrossCutting.Security.Authorization;
using Domain.Prospecting.Entities;
using LeadManagerApi.Core.ApiFeatures;
using LeadManagerApi.Prospecting.Leads.Core;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shared.DataPagination;
using Shared.Results;
using static Application.Security.LeadManagerSecurityConfiguration;

namespace LeadManagerApi.Prospecting.Leads.GetLeads;

[LeadsRoute]
[RequiredAllPermissions(Permissions.Read)]
public sealed class GetLeadsController : LeadManagerController
{
    public GetLeadsController(ISender sender) : base(sender) { }

    [HttpGet]
    [ProducesResponseType(typeof(ApplicationResponse<PagedList<LeadDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApplicationResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApplicationResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetLeadsAsync([FromQuery] PaginationOptions paginationOptions, CancellationToken cancellationToken)
    {
        var response = await Mediator.Send(new GetLeadsQueryRequest(paginationOptions), cancellationToken);

        return Result(response);
    }
}