using Application.Features.Addresses.Queries.SearchAddressByZipCode;
using LeadManagerApi.ApiFeatures;
using LeadManagerApi.Configuration.Security;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Results;

namespace LeadManagerApi.Addresses;

[LeadManagerApiRoute("addresses")]
[Authorize(Policy = LeadManagerApiSecurityConfiguration.Policies.LeadManagerDefaultPolicy)]
public sealed class SearchAddressByZipCodeController : LeadManagerController
{
    public SearchAddressByZipCodeController(ISender sender) : base(sender) { }

    [HttpGet("search")]
    [ProducesResponseType(typeof(ApplicationResponse<SearchAddressByZipCodeQueryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApplicationResponse<SearchAddressByZipCodeQueryResponse>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApplicationResponse<SearchAddressByZipCodeQueryResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApplicationResponse<SearchAddressByZipCodeQueryResponse>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApplicationResponse<SearchAddressByZipCodeQueryResponse>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApplicationResponse<SearchAddressByZipCodeQueryResponse>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> SearchAddressByZipCodeAsync(string cep, CancellationToken cancellationToken)
    {
        var response = await Mediator.Send(new SearchAddressByZipCodeQueryRequest(cep), cancellationToken);

        return Result(response);
    }
}