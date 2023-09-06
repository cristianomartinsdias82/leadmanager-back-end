using MediatR;
using Shared.Events.DomainEvents;

namespace Core.DomainEvents.LeadRegistered;

internal sealed class LeadRegisteredDomainEventHandler : ApplicationDomainEventHandler<LeadRegisteredDomainEvent>
{
    public LeadRegisteredDomainEventHandler(
        IMediator mediator) : base(mediator)
    {
    }

    public override async Task Handle(LeadRegisteredDomainEvent notification, CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
    }
}
