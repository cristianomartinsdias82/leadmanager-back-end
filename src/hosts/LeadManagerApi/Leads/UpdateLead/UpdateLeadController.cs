using Application.Features.Leads.Commands.UpdateLead;
using LeadManagerApi.ApiFeatures;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shared.Results;

namespace LeadManagerApi.Leads.UpdateLead;

[LeadsRoute]
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