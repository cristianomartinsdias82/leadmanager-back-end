using Application.Contracts.Caching;
using MediatR;
using Shared.RequestHandling;
using Shared.Results;

namespace Application.Features.Leads.Commands.ClearLeadsCache;

internal sealed class ClearLeadsCacheCommandHandler : ApplicationRequestHandler<ClearLeadsCacheCommandRequest, ClearLeadsCacheCommandResponse>
{
    private readonly ICachingManagement _cachingManager;

    public ClearLeadsCacheCommandHandler(
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