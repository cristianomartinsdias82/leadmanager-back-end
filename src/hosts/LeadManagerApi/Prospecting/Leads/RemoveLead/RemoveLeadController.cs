using Application.Prospecting.Leads.Commands.RemoveLead;
using CrossCutting.Security.Authorization;
using LeadManagerApi.Core.ApiFeatures;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shared.Results;
using static Application.Security.LeadManagerSecurityConfiguration;

namespace LeadManagerApi.Prospecting.Leads.RemoveLead;

[LeadsRoute]
[RequiredAllPermissions(Permissions.Delete, Order = 1)]
[RequiresOneTimePassword(Order = 2)]
public sealed class RemoveLeadController : LeadManagerController
{
    public RemoveLeadController(ISender sender) : base(sender) { }

    [HttpDelete("{id:Guid}")]
    [ProducesResponseType(typeof(ApplicationResponse<RemoveLeadCommandResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApplicationResponse<RemoveLeadCommandResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApplicationResponse<RemoveLeadCommandResponse>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> RemoveLeadAsync(
        Guid id,
        [FromQuery] byte[] revision,
        CancellationToken cancellationToken)
    {
        var response = await Mediator.Send(new RemoveLeadCommandRequest { Id = id, Revision = revision }, cancellationToken);

        return Result(response);
    }
}