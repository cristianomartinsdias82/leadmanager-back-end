using Application.Features.Leads.Commands.RemoveLead;
using CrossCutting.Security.Authorization;
using LeadManagerApi.ApiFeatures;
using LeadManagerApi.Core.ApiFeatures;
using LeadManagerApi.Core.Configuration.Security;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shared.Results;

namespace LeadManagerApi.Leads.RemoveLead;

[LeadsRoute]
//[Authorize(Policy = LeadManagerApiSecurityConfiguration.Policies.LeadManagerRemovePolicy)]
[RequiredAllPermissions(requiredPermissions: LeadManagerApiSecurityConfiguration.Claims.Delete)]
public sealed class RemoveLeadController : LeadManagerController
{
    public RemoveLeadController(ISender sender) : base(sender) { }

    [HttpDelete("{id:Guid}")]
    [ProducesResponseType(typeof(ApplicationResponse<RemoveLeadCommandResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApplicationResponse<RemoveLeadCommandResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApplicationResponse<RemoveLeadCommandResponse>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> RemoveLeadAsync(
        Guid id,
        [FromQuery]byte[] revision,
        CancellationToken cancellationToken)
    {
        var response = await Mediator.Send(new RemoveLeadCommandRequest { Id = id, Revision = revision }, cancellationToken);

        return Result(response);
    }
}