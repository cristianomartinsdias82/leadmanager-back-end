using Application.Contracts.Caching;
using Application.Contracts.Persistence;
using Application.Features.Leads.IntegrationEvents.LeadRemoved;
using Application.Features.Leads.Shared;
using Core.DomainEvents.LeadRemoved;
using MediatR;
using Shared.Events.EventDispatching;
using Shared.RequestHandling;
using Shared.Results;

namespace Application.Features.Leads.Commands.RemoveLead;

internal sealed class RemoveLeadCommandHandler : ApplicationRequestHandler<RemoveLeadCommandRequest, RemoveLeadCommandResponse>
{
    private readonly ILeadManagerDbContext _dbContext;
    private readonly ICachingManagement _cachingManager;

    public RemoveLeadCommandHandler(
        IMediator mediator,
        IEventDispatching eventDispatcher,
        ILeadManagerDbContext dbContext,
        ICachingManagement cachingManager) : base(mediator, eventDispatcher)
    {
        _dbContext = dbContext;
        _cachingManager = cachingManager;
    }

    public async override Task<ApplicationResponse<RemoveLeadCommandResponse>> Handle(RemoveLeadCommandRequest request, CancellationToken cancellationToken)
    {
        var lead = await _dbContext.Leads.FindAsync(request.Id);
        if (lead is null)
            return ApplicationResponse<RemoveLeadCommandResponse>.Create(default!, message: "Lead não encontrado.");

        _dbContext.Leads.Remove(lead);

        await _dbContext.SaveChangesAsync(cancellationToken);

        var leadDto = lead.ToDto();
        await _cachingManager.RemoveLeadEntryAsync(leadDto, cancellationToken);

        AddEvent(new LeadRemovedDomainEvent(lead));
        AddEvent(new LeadRemovedIntegrationEvent(leadDto));

        return ApplicationResponse<RemoveLeadCommandResponse>.Create(new RemoveLeadCommandResponse());
    }
}