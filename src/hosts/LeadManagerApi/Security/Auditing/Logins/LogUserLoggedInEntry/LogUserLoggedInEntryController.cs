using Application.Security.Auditing.Logins.Commands.LogUserLoggedInEntry;
using LeadManagerApi.Core.ApiFeatures;
using LeadManagerApi.Security.Auditing.Core;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shared.Results;

namespace LeadManagerApi.Security.Auditing.Logins.LogUserLoggedInEntry;

[AuditingRoute]
public sealed class LogUserLoggedInEntryController : LeadManagerController
{
    public LogUserLoggedInEntryController(ISender sender) : base(sender) { }

    [HttpPost("logins/log")]
    [ProducesResponseType(typeof(ApplicationResponse<LogUserLoggedInEntryCommandResponse>), StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApplicationResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> LogUserLoggedInEntryAsync(CancellationToken cancellationToken)
    {
        var response = await Mediator.Send(new LogUserLoggedInEntryCommandRequest(), cancellationToken);

        return Result(
            response,
            onSuccessStatusCodeFactory: (_, _) => StatusCodes.Status204NoContent);
    }
}