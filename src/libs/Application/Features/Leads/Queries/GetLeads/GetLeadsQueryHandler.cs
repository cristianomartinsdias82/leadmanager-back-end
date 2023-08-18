using Application.Contracts.Caching;
using Application.Features.Leads.Shared;
using MediatR;
using Shared.RequestHandling;
using Shared.Results;

namespace Application.Features.Leads.Queries.GetLeads;

internal sealed class GetLeadsQueryHandler : ApplicationRequestHandler<GetLeadsQueryRequest, IEnumerable<LeadDto>>
{
    private readonly ICachingManagement _cachingManager;

    public GetLeadsQueryHandler(
        IMediator mediator,
        ICachingManagement cachingManager) : base(mediator, default!)
    {
        _cachingManager = cachingManager;
    }

    public async override Task<ApplicationResponse<IEnumerable<LeadDto>>> Handle(GetLeadsQueryRequest request, CancellationToken cancellationToken)
    {
        var cachedLeads = await _cachingManager.GetLeadsAsync(cancellationToken);

        return ApplicationResponse<IEnumerable<LeadDto>>.Create(cachedLeads);
    }
}