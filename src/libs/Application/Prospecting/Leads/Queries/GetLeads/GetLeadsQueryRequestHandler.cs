using Application.Core.Contracts.Repository.Caching;
using Domain.Prospecting.Entities;
using MediatR;
using Shared.DataQuerying;
using Shared.RequestHandling;
using Shared.Results;

namespace Application.Prospecting.Leads.Queries.GetLeads;

internal sealed class GetLeadsQueryRequestHandler : ApplicationRequestHandler<GetLeadsQueryRequest, PagedList<LeadDto>>
{
    private readonly ICachingLeadRepository _cachingLeadRepository;

    public GetLeadsQueryRequestHandler(
        IMediator mediator,
		ICachingLeadRepository cachingLeadRepository) : base(mediator, default!)
    {
        _cachingLeadRepository = cachingLeadRepository;
    }

    public async override Task<ApplicationResponse<PagedList<LeadDto>>> Handle(
        GetLeadsQueryRequest request,
        CancellationToken cancellationToken)
    {
        var pagedLeads = await _cachingLeadRepository.GetAsDtoAsync(
            request.PaginationOptions,
            new QueryOptions { Term = request.Search },
            cancellationToken);

        return ApplicationResponse<PagedList<LeadDto>>.Create(pagedLeads);
    }
}
