using MediatR;
using Shared.Events.DomainEvents;

namespace Core.DomainEvents.LeadRemoved;

internal sealed class LeadRemovedDomainEventHandler : ApplicationDomainEventHandler<LeadRemovedDomainEvent>
{
    public LeadRemovedDomainEventHandler(
        IMediator mediator) : base(mediator)
    {
    }

    public override async Task Handle(LeadRemovedDomainEvent notification, CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
    }
}
