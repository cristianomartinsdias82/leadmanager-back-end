using Core.Entities.DomainEvents;
using MediatR;

namespace Core.DomainEvents.LeadDataUpdated;

internal sealed class LeadDataUpdatedDomainEventHandler : INotificationHandler<LeadDataUpdatedDomainEvent>
{
    private readonly IMediator _mediator;

    public LeadDataUpdatedDomainEventHandler(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task Handle(LeadDataUpdatedDomainEvent notification, CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
    }
}
