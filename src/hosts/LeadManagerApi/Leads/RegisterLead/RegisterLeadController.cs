using Application.Features.Leads.Commands.RegisterLead;
using LeadManagerApi.ApiFeatures;
using LeadManagerApi.Configuration.Security;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Results;

namespace LeadManagerApi.Leads.RegisterLead;

[LeadsRoute]
[Authorize(Policy = LeadManagerApiSecurityConfiguration.Policies.LeadManagerDefaultPolicy)]
public sealed class RegisterLeadController : LeadManagerController
{
    public RegisterLeadController(ISender mediator) : base(mediator) { }

    [HttpPost]
    [ProducesResponseType(typeof(ApplicationResponse<Guid>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApplicationResponse<Guid>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> RegisterLeadAsync([FromBody]RegisterLeadCommandRequest request, CancellationToken cancellationToken)
    {
        var response = await Mediator.Send(request, cancellationToken);

        return Result(
            response,
            onSuccessStatusCodeFactory: (_, response) => StatusCodes.Status201Created);
    }
}