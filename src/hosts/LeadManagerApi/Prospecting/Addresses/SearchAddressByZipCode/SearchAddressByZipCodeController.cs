using Application.Prospecting.Addresses.Queries.SearchAddressByZipCode;
using CrossCutting.Security.Authorization;
using LeadManagerApi.Core.ApiFeatures;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shared.Results;
using static Application.Security.LeadManagerSecurityConfiguration;

namespace LeadManagerApi.Prospecting.Addresses.SearchAddressByZipCode;

[LeadManagerApiRoute("addresses")]
[RequiredAllPermissions(Permissions.Read)]
public sealed class SearchAddressByZipCodeController : LeadManagerController
{
    public SearchAddressByZipCodeController(ISender sender) : base(sender) { }

    [HttpGet("search")]
    [ProducesResponseType(typeof(ApplicationResponse<SearchAddressByZipCodeQueryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApplicationResponse<SearchAddressByZipCodeQueryResponse>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApplicationResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> SearchAddressByZipCodeAsync(string cep, CancellationToken cancellationToken)
    {
        var response = await Mediator.Send(new SearchAddressByZipCodeQueryRequest(cep), cancellationToken);

        return Result(response);
    }
}