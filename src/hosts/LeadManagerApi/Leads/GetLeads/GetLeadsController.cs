using Application.Features.Leads.Queries.GetLeads;
using Application.Features.Leads.Queries.Shared;
using LeadManagerApi.ApiFeatures;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shared;

namespace LeadManagerApi.Leads.GetLeads;

[LeadsRoute]
public sealed class GetLeadsController : LeadManagerController
{
    public GetLeadsController(ISender sender) : base(sender) { }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<LeadDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetLeadsAsync([FromQuery]PaginationOptions paginationOptions, CancellationToken cancellationToken)
    {
        var response = await Mediator.Send(new GetLeadsQueryRequest(paginationOptions), cancellationToken);

        return Result(response);
    }
}