using Application.Prospecting.Leads.Queries.GetLeads;
using CrossCutting.Security.Authorization;
using Domain.Prospecting.Entities;
using LeadManagerApi.Core.ApiFeatures;
using LeadManagerApi.Core.Configuration.Caching;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
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
    [OutputCache(PolicyName=LeadManagerApiCachingConfiguration.Policies.Get.Name)]
    [ProducesResponseType(typeof(ApplicationResponse<PagedList<LeadDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetLeadsAsync(
        [FromQuery] string? search,
        [FromQuery] PaginationOptions paginationOptions, CancellationToken cancellationToken)
    {
	    var response = await Mediator.Send(new GetLeadsQueryRequest(search, paginationOptions), cancellationToken);

        return Result(response);
    }
}