using Application.Features.Leads.Commands.BulkInsertLead;
using CrossCutting.Security.Authorization;
using LeadManagerApi.Core.ApiFeatures;
using LeadManagerApi.Core.Configuration;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shared.Results;
using static LeadManagerApi.Core.Configuration.Security.LeadManagerApiSecurityConfiguration;

namespace LeadManagerApi.Leads.RegisterLead;

[LeadsRoute]
//[Authorize(Policy = LeadManagerApiSecurityConfiguration.Policies.LeadManagerDefaultPolicy)]
[RequiredAllPermissions(Permissions.BulkInsert)]
public sealed class BulkInsertLeadController : LeadManagerController
{
    public BulkInsertLeadController(ISender mediator) : base(mediator) { }

    [HttpPost("bulk-insert")]
    [ProducesResponseType(typeof(ApplicationResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApplicationResponse<bool>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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