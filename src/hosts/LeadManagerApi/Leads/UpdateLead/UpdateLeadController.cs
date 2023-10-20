using Application.Features.Leads.Commands.UpdateLead;
using CrossCutting.Security.Authorization;
using LeadManagerApi.Core.ApiFeatures;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shared.Results;
using static LeadManagerApi.Core.Configuration.Security.LeadManagerApiSecurityConfiguration;

namespace LeadManagerApi.Leads.UpdateLead;

[LeadsRoute]
//[Authorize(Policy = LeadManagerApiSecurityConfiguration.Policies.LeadManagerDefaultPolicy)]
[RequiredAllPermissions(Permissions.Update)]
public sealed class UpdateLeadController : LeadManagerController
{
    public UpdateLeadController(ISender mediator) : base(mediator) { }

    [HttpPut("{id:Guid}")]
    [ProducesResponseType(typeof(ApplicationResponse<UpdateLeadCommandResponse>), StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApplicationResponse<UpdateLeadCommandResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApplicationResponse<UpdateLeadCommandResponse>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateLeadAsync(Guid id, [FromBody]UpdateLeadCommandRequest request, CancellationToken cancellationToken)
    {
        request.Id ??= id;

        var response = await Mediator.Send(request, cancellationToken);

        return Result(response);
    }
}