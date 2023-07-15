using Application.Features.Leads.Queries.SearchLead;
using LeadManagerApi.ApiFeatures;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shared.Results;

namespace LeadManagerApi.Leads.SearchLead
{
    [LeadsRoute]
    public sealed class SearchLeadController : LeadManagerController
    {
        public SearchLeadController(ISender mediator) : base(mediator) { }

        [HttpGet("search")]
        [ProducesResponseType(typeof(ApplicationResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApplicationResponse<bool>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApplicationResponse<bool>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApplicationResponse<bool>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApplicationResponse<bool>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SearchLead([FromQuery] string cnpjRazaoSocial, CancellationToken cancellationToken)
        {
            var response = await Mediator.Send(new SearchLeadQueryRequest(cnpjRazaoSocial), cancellationToken);

            return Result(response);
        }
    }
}