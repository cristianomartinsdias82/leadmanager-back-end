using Application.Prospecting.Leads.Commands.BulkInsertLead;
using CrossCutting.Security.Authorization;
using LeadManagerApi.Core.ApiFeatures;
using LeadManagerApi.Core.Configuration;
using LeadManagerApi.Prospecting.Leads.Core;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shared.Results;
using static Application.Security.LeadManagerSecurityConfiguration;

namespace LeadManagerApi.Prospecting.Leads.BulkInsertLead;

[LeadsRoute]
[RequiredAllPermissions(Permissions.BulkInsert)]
public sealed class BulkInsertLeadController : LeadManagerController
{
    public BulkInsertLeadController(ISender mediator) : base(mediator) { }

    [HttpPost("bulk-insert")]
    [ProducesResponseType(typeof(ApplicationResponse<BulkInsertLeadCommandResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApplicationResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApplicationResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApplicationResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> BulkInsertLeadAsync(IFormFile file, CancellationToken cancellationToken)
    {
        var request = new BulkInsertLeadCommandRequest
        {
            InputStream = file?.OpenReadStream() ?? default!,
            FileUpload_MaxSizeInBytesConfigurationKeyName = nameof(LeadManagerApiSettings.FileUpload_MaxSizeInBytes),
            ContentSizeInBytes = file?.Length ?? 0L,
            ContentType = file?.ContentType ?? default!,
            FileName = file?.FileName ?? default!
        };

        var response = await Mediator.Send(request, cancellationToken);

        return Result(response);
    }
}