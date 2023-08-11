using MediatR;

namespace Shared.Events.IntegrationEvents;

public abstract class ApplicationIntegrationEventHandler<TIntegrationEvent> : INotificationHandler<TIntegrationEvent>
    where TIntegrationEvent : IIntegrationEvent
{
    protected IMediator Mediator { get; }

    public ApplicationIntegrationEventHandler(IMediator mediator)
    {
        ArgumentNullException.ThrowIfNull(mediator);

        Mediator = mediator;
    }

    public abstract Task Handle(TIntegrationEvent notification, CancellationToken cancellationToken);
}