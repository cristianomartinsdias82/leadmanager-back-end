using Application.Prospecting.Leads.Commands.RegisterLead;
using CrossCutting.Security.Authorization;
using LeadManagerApi.Core.ApiFeatures;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shared.Results;
using static Application.Security.LeadManagerSecurityConfiguration;

namespace LeadManagerApi.Prospecting.Leads.RegisterLead;

[LeadsRoute]
[RequiredAllPermissions(Permissions.Insert)]
public sealed class RegisterLeadController : LeadManagerController
{
    public RegisterLeadController(ISender mediator) : base(mediator) { }

    [HttpPost]
    [ProducesResponseType(typeof(ApplicationResponse<RegisterLeadCommandResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApplicationResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> RegisterLeadAsync([FromBody] RegisterLeadCommandRequest request, CancellationToken cancellationToken)
    {
        var response = await Mediator.Send(request, cancellationToken);

        return Result(
            response,
            onSuccessStatusCodeFactory: (_, _) => StatusCodes.Status201Created,
            routeData: ("leads", response.Data.Id));
    }
}