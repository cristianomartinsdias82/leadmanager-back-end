namespace Shared.Events.EventDispatching;

public interface IEventDispatching
{
    void AddEvent(IEvent @event);
    Task DispatchEvents(CancellationToken cancellationToken = default);
    void ClearEvents();
    bool HasEvents { get; }
}