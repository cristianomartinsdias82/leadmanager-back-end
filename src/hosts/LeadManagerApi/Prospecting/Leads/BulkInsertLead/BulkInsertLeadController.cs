using Application.Prospecting.Leads.Commands.BulkInsertLead;
using CrossCutting.Security.Authorization;
using LeadManagerApi.Core.ApiFeatures;
using LeadManagerApi.Core.Configuration;
//using LeadManagerApi.Core.Configuration.Caching;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Shared.Results;
using static Application.Security.LeadManagerSecurityConfiguration;

namespace LeadManagerApi.Prospecting.Leads.BulkInsertLead;

[LeadsRoute]
[RequiredAllPermissions(Permissions.BulkInsert)]
public sealed class BulkInsertLeadController : LeadManagerController
{
	//private readonly IOutputCacheStore _outputCacheStore;

	public BulkInsertLeadController(
        ISender mediator
        /*,IOutputCacheStore outputCacheStore*/) : base(mediator)
	{
		//_outputCacheStore = outputCacheStore;
	}

	[HttpPost("bulk-insert")]
    [ProducesResponseType(typeof(ApplicationResponse<BulkInsertLeadCommandResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApplicationResponse<object>), StatusCodes.Status400BadRequest)]
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

		//(Output Cache) Keep it commented out (demonstration purposes of how to apply in projects)
		//if (response.Success)
        //  await _outputCacheStore.EvictByTagAsync(
		//				LeadManagerApiCachingConfiguration.Policies.Get.Tag,
		//				cancellationToken);

        return Result(response);
    }
}