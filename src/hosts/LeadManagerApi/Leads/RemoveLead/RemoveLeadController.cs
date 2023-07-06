using Application.Features.Leads.Commands.RemoveLead;
using LeadManagerApi.ApiFeatures;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shared.Results;

namespace LeadManagerApi.Leads.RemoveLead;

[LeadsRoute]
public sealed class RemoveLeadController : LeadManagerController
{
    public RemoveLeadController(ISender sender) : base(sender) { }

    [HttpDelete("{id:Guid}")]
    [ProducesResponseType(typeof(ApplicationResponse<Guid>), StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApplicationResponse<Guid>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApplicationResponse<Guid>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> RemoveLeadAsync(Guid id, CancellationToken cancellationToken)
    {
        var response = await Mediator.Send(new RemoveLeadCommandRequest { Id = id }, cancellationToken);

        return Result(
            response,
            onSuccessStatusCodeFactory: (_, response) => StatusCodes.Status204NoContent);
    }
}