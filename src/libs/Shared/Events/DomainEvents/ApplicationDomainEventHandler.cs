using MediatR;

namespace Shared.Events.DomainEvents;

public abstract class ApplicationDomainEventHandler<TDomainEvent> : INotificationHandler<TDomainEvent>
    where TDomainEvent : IDomainEvent
{
    protected IMediator Mediator { get; }

    public ApplicationDomainEventHandler(IMediator mediator)
    {
        Mediator = mediator;
    }

    public abstract Task Handle(TDomainEvent notification, CancellationToken cancellationToken);
}