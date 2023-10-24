using Application.Core.Contracts.Caching;
using MediatR;
using Shared.RequestHandling;
using Shared.Results;

namespace Application.Prospecting.Leads.Commands.ClearLeadsCache;

internal sealed class ClearLeadsCacheCommandRequestHandler : ApplicationRequestHandler<ClearLeadsCacheCommandRequest, ClearLeadsCacheCommandResponse>
{
    private readonly ICachingManagement _cachingManager;

    public ClearLeadsCacheCommandRequestHandler(
        IMediator mediator,
        ICachingManagement cachingManager
        ) : base(mediator, default!)
    {
        _cachingManager = cachingManager;
    }

    public async override Task<ApplicationResponse<ClearLeadsCacheCommandResponse>> Handle(ClearLeadsCacheCommandRequest request, CancellationToken cancellationToken)
    {
        await _cachingManager.ClearLeadEntriesAsync(cancellationToken);

        return ApplicationResponse<ClearLeadsCacheCommandResponse>.Create(new());
    }
}