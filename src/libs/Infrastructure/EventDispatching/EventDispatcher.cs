using MediatR;
using Shared.Events;
using Shared.Events.DomainEvents;
using Shared.Events.EventDispatching;
using Shared.Events.IntegrationEvents;
using Shared.Helpers;

namespace Infrastructure.EventDispatching;

internal sealed class EventDispatcher : IEventDispatching
{
    private readonly IPublisher _publisher;
    private Queue<IDomainEvent> _domainEvents = new Queue<IDomainEvent>();
    private Queue<IIntegrationEvent> _integrationEvents = new Queue<IIntegrationEvent>();

    public EventDispatcher(IPublisher publisher)
    {
        _publisher = publisher;
    }

    public void AddEvent(IEvent @event)
    {
        ArgumentNullException.ThrowIfNull(@event);

        if (@event is IDomainEvent)
            _domainEvents.Enqueue((@event as IDomainEvent)!);
        else
            _integrationEvents.Enqueue((@event as IIntegrationEvent)!);
    }

    public Task DispatchEvents(CancellationToken cancellationToken = default)
    {
        return TaskHelper.WhenAll(
            Task.Run(() =>
            {
                while (_domainEvents.Any())
                    _publisher.Publish(_domainEvents.Dequeue(), cancellationToken);
            }),
            Task.Run(() =>
            {
                while (_integrationEvents.Any())
                    _publisher.Publish(_integrationEvents.Dequeue(), cancellationToken);
            })
        );
    }

    public void ClearEvents()
    {
        _domainEvents.Clear();
        _integrationEvents.Clear();
    }

    public bool HasEvents
        => _domainEvents.Count > 0 || _integrationEvents.Count > 0;
}
