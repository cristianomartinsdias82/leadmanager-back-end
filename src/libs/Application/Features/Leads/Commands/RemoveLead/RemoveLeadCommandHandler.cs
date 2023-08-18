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

    public RemoveLeadCommandHandler(
        IMediator mediator,
        IEventDispatching eventDispatcher,
        ILeadManagerDbContext dbContext) : base(mediator, eventDispatcher)
    {
        _dbContext = dbContext;
    }

    public async override Task<ApplicationResponse<RemoveLeadCommandResponse>> Handle(RemoveLeadCommandRequest request, CancellationToken cancellationToken)
    {
        var lead = await _dbContext.Leads.FindAsync(request.Id);
        if (lead is null)
            return ApplicationResponse<RemoveLeadCommandResponse>.Create(default!, message: "Lead não encontrado.");

        _dbContext.Leads.Remove(lead);

        await _dbContext.SaveChangesAsync(cancellationToken);

        AddEvent(new LeadRemovedDomainEvent(lead));
        AddEvent(new LeadRemovedIntegrationEvent(lead.ToDto()));

        return ApplicationResponse<RemoveLeadCommandResponse>.Create(new RemoveLeadCommandResponse());
    }
}