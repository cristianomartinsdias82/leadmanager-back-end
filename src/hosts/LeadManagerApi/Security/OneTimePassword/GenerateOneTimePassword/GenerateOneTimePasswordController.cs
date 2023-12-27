using Application.Security.OneTimePassword.Commands.GenerateOneTimePassword;
using LeadManagerApi.Core.ApiFeatures;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shared.Results;

namespace LeadManagerApi.Security.OneTimePassword.GenerateNewPassword;

[LeadManagerApiRoute("one-time-password")]
public sealed class GenerateOneTimePasswordController : LeadManagerController
{
    public GenerateOneTimePasswordController(ISender mediator) : base(mediator)
    {
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApplicationResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApplicationResponse<bool>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GenerateNewOneTimePasswordAsync([FromBody] GenerateOneTimePasswordCommandRequest request, CancellationToken cancellationToken)
    {
        var response = await Mediator.Send(request, cancellationToken);

        return Result(response);
    }
}