using MediatR;
using Shared.Events.DomainEvents;

namespace Domain.Prospecting.DomainEvents.LeadUpdated;

internal sealed class LeadUpdatedDomainEventHandler : ApplicationDomainEventHandler<LeadUpdatedDomainEvent>
{
    public LeadUpdatedDomainEventHandler(
        IMediator mediator) : base(mediator)
    {
    }

    public override async Task Handle(LeadUpdatedDomainEvent notification, CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
    }
}
