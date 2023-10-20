using Application.Features.Leads.Queries.GetLeadById;
using Application.Features.Leads.Shared;
using CrossCutting.Security.Authorization;
using LeadManagerApi.Core.ApiFeatures;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shared.Results;
using static LeadManagerApi.Core.Configuration.Security.LeadManagerApiSecurityConfiguration;

namespace LeadManagerApi.Leads.GetLeadById;

[LeadsRoute]
//[Authorize(Policy = LeadManagerApiSecurityConfiguration.Policies.LeadManagerDefaultPolicy)]
[RequiredAllPermissions(Permissions.Read)]
public sealed class GetLeadByIdController : LeadManagerController
{
    public GetLeadByIdController(ISender sender) : base(sender) { }

    [HttpGet("{id:Guid}")]
    [ProducesResponseType(typeof(ApplicationResponse<LeadDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApplicationResponse<Guid>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApplicationResponse<Guid>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetLeadByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var response = await Mediator.Send(new GetLeadByIdQueryRequest { Id = id }, cancellationToken);

        return Result(response);
    }
}