using Application.Prospecting.Leads.Commands.RegisterLead;
using CrossCutting.Security.Authorization;
using LeadManagerApi.Core.ApiFeatures;
//using LeadManagerApi.Core.Configuration.Caching;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Shared.Results;
using static Application.Security.LeadManagerSecurityConfiguration;

namespace LeadManagerApi.Prospecting.Leads.RegisterLead;

[LeadsRoute]
[RequiredAllPermissions(Permissions.Insert)]
public sealed class RegisterLeadController : LeadManagerController
{
	//private readonly IOutputCacheStore _outputCacheStore;

	public RegisterLeadController(
        ISender mediator
        /*,IOutputCacheStore outputCacheStore*/) : base(mediator)
    {
        //_outputCacheStore = outputCacheStore;
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApplicationResponse<RegisterLeadCommandResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApplicationResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> RegisterLeadAsync([FromBody] RegisterLeadCommandRequest request, CancellationToken cancellationToken)
    {
        var response = await Mediator.Send(request, cancellationToken);

		//(Output Cache) Keep it commented out (demonstration purposes of how to apply in projects)
		//if (response.Success)
		//	await _outputCacheStore.EvictByTagAsync(
		//				LeadManagerApiCachingConfiguration.Policies.Get.Tag,
		//				cancellationToken);

		return Result(
            response,
            onSuccessStatusCodeFactory: (_, _) => StatusCodes.Status201Created,
            routeData: ("leads", response.Data.Id));
    }
}