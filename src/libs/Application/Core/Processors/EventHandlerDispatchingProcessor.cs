using MediatR;
using MediatR.Pipeline;
using Shared.Events.EventDispatching;

namespace Application.Core.Processors;

internal sealed class EventHandlerDispatchingProcessor<TRequest, TResponse> : IRequestPostProcessor<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEventDispatching _eventDispatcher;

    public EventHandlerDispatchingProcessor(IEventDispatching eventDispatcher)
    {
        _eventDispatcher = eventDispatcher;
    }

    public async Task Process(TRequest request, TResponse response, CancellationToken cancellationToken)
    {
        if (_eventDispatcher.HasEvents)
            await _eventDispatcher.DispatchEvents(cancellationToken);
    }
}