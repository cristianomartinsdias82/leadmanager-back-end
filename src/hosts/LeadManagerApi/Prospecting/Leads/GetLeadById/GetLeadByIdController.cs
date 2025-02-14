using Application.Prospecting.Leads.Queries.GetLeadById;
using CrossCutting.Security.Authorization;
using Domain.Prospecting.Entities;
using LeadManagerApi.Core.ApiFeatures;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shared.Results;
using static Application.Security.LeadManagerSecurityConfiguration;

namespace LeadManagerApi.Prospecting.Leads.GetLeadById;

[LeadsRoute]
[RequiredAllPermissions(Permissions.Read)]
public sealed class GetLeadByIdController : LeadManagerController
{
    public GetLeadByIdController(ISender sender) : base(sender) { }

    [HttpGet("{id:Guid}")]
    [ProducesResponseType(typeof(ApplicationResponse<LeadDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApplicationResponse<LeadDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApplicationResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetLeadByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var response = await Mediator.Send(new GetLeadByIdQueryRequest { Id = id }, cancellationToken);

        return Result(response);
    }
}