using Application.Prospecting.Leads.Commands.UpdateLead;
using CrossCutting.Security.Authorization;
using LeadManagerApi.Core.ApiFeatures;
using LeadManagerApi.Core.Configuration.Caching;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Shared.Results;
using static Application.Security.LeadManagerSecurityConfiguration;

namespace LeadManagerApi.Prospecting.Leads.UpdateLead;

[LeadsRoute]
[RequiredAllPermissions(Permissions.Update)]
public sealed class UpdateLeadController : LeadManagerController
{
	private readonly IOutputCacheStore _outputCacheStore;

	public UpdateLeadController(
		ISender mediator,
		IOutputCacheStore outputCacheStore) : base(mediator)
	{
		_outputCacheStore = outputCacheStore;
	}

	[HttpPut("{id:Guid}")]
    [ProducesResponseType(typeof(ApplicationResponse<UpdateLeadCommandResponse>), StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApplicationResponse<UpdateLeadCommandResponse>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApplicationResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateLeadAsync(Guid id, [FromBody] UpdateLeadCommandRequest request, CancellationToken cancellationToken)
    {
        request.Id ??= id;

        var response = await Mediator.Send(request, cancellationToken);

		if (response.Success)
			await _outputCacheStore.EvictByTagAsync(
						LeadManagerApiCachingConfiguration.Policies.Get.Tag,
						cancellationToken);

		return Result(
            response,
            onSuccessStatusCodeFactory: (_, _) => StatusCodes.Status204NoContent);
    }
}