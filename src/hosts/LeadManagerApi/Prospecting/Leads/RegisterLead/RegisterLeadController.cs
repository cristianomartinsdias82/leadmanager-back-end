using Application.Prospecting.Leads.Commands.RegisterLead;
using CrossCutting.Security.Authorization;
using LeadManagerApi.Core.ApiFeatures;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shared.Results;
using static LeadManagerApi.Core.Configuration.Security.LeadManagerApiSecurityConfiguration;

namespace LeadManagerApi.Prospecting.Leads.RegisterLead;

[LeadsRoute]
//[Authorize(Policy = LeadManagerApiSecurityConfiguration.Policies.LeadManagerDefaultPolicy)]
[RequiredAllPermissions(Permissions.Insert)]
public sealed class RegisterLeadController : LeadManagerController
{
    public RegisterLeadController(ISender mediator) : base(mediator) { }

    [HttpPost]
    [ProducesResponseType(typeof(ApplicationResponse<Guid>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApplicationResponse<Guid>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> RegisterLeadAsync([FromBody] RegisterLeadCommandRequest request, CancellationToken cancellationToken)
    {
        var response = await Mediator.Send(request, cancellationToken);

        return Result(
            response,
            onSuccessStatusCodeFactory: (_, response) => StatusCodes.Status201Created);
    }
}