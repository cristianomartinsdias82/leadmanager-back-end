using Application.Features.Leads.Commands.BulkInsertLead;
using LeadManagerApi.ApiFeatures;
using LeadManagerApi.Configuration;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shared.Results;

namespace LeadManagerApi.Leads.RegisterLead;

[LeadsRoute]
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