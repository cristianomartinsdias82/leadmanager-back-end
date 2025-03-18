using Application.Prospecting.Leads.Commands.RemoveLead;
using CrossCutting.Security.Authorization;
using LeadManagerApi.Core.ApiFeatures;
using LeadManagerApi.Core.Configuration.Caching;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Shared.Results;
using static Application.Security.LeadManagerSecurityConfiguration;

namespace LeadManagerApi.Prospecting.Leads.RemoveLead;

[LeadsRoute]
[RequiredAllPermissions(Permissions.Delete, Order = 1)]
[RequiresOneTimePassword(Order = 2)]
public sealed class RemoveLeadController : LeadManagerController
{
	private readonly IOutputCacheStore _outputCacheStore;

    public RemoveLeadController(
		ISender mediator,
		IOutputCacheStore outputCacheStore) : base(mediator)
	{
		_outputCacheStore = outputCacheStore;
	}

    [HttpDelete("{id:Guid}")]
    [ProducesResponseType(typeof(ApplicationResponse<RemoveLeadCommandResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApplicationResponse<RemoveLeadCommandResponse>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApplicationResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> RemoveLeadAsync(
        Guid id,
        [FromQuery] byte[] revision,
        CancellationToken cancellationToken)
    {
        var response = await Mediator.Send(new RemoveLeadCommandRequest { Id = id, Revision = revision }, cancellationToken);

		if (response.Success)
			await _outputCacheStore.EvictByTagAsync(
						LeadManagerApiCachingConfiguration.Policies.Get.Tag,
						cancellationToken);

		return Result(
            response,
            onSuccessStatusCodeFactory: (_, _) => StatusCodes.Status204NoContent);
    }
}