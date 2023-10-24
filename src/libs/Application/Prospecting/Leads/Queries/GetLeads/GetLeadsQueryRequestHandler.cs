using Application.Core.Contracts.Caching;
using Application.Prospecting.Leads.Shared;
using MediatR;
using Shared.DataPagination;
using Shared.RequestHandling;
using Shared.Results;

namespace Application.Prospecting.Leads.Queries.GetLeads;

internal sealed class GetLeadsQueryRequestHandler : ApplicationRequestHandler<GetLeadsQueryRequest, PagedList<LeadDto>>
{
    private readonly ICachingManagement _cachingManager;

    public GetLeadsQueryRequestHandler(
        IMediator mediator,
        ICachingManagement cachingManager) : base(mediator, default!)
    {
        _cachingManager = cachingManager;
    }

    public async override Task<ApplicationResponse<PagedList<LeadDto>>> Handle(
        GetLeadsQueryRequest request,
        CancellationToken cancellationToken)
    {
        var cachedLeads = await _cachingManager.GetLeadsAsync(
            request.PaginationOptions,
            cancellationToken);

        return ApplicationResponse<PagedList<LeadDto>>.Create(cachedLeads);
    }
}
