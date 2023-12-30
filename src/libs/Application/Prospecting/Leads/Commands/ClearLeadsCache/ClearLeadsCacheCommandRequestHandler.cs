using Application.Core.Contracts.Repository.Caching;
using MediatR;
using Shared.RequestHandling;
using Shared.Results;

namespace Application.Prospecting.Leads.Commands.ClearLeadsCache;

internal sealed class ClearLeadsCacheCommandRequestHandler : ApplicationRequestHandler<ClearLeadsCacheCommandRequest, ClearLeadsCacheCommandResponse>
{
    private readonly ICachingLeadRepository _cachingLeadRepository;

    public ClearLeadsCacheCommandRequestHandler(
        IMediator mediator,
        ICachingLeadRepository cachingLeadRepository
        ) : base(mediator, default!)
    {
        _cachingLeadRepository = cachingLeadRepository;
    }

    public async override Task<ApplicationResponse<ClearLeadsCacheCommandResponse>> Handle(ClearLeadsCacheCommandRequest request, CancellationToken cancellationToken)
    {
        await _cachingLeadRepository.ClearAsync(cancellationToken);

        return ApplicationResponse<ClearLeadsCacheCommandResponse>.Create(new());
    }
}