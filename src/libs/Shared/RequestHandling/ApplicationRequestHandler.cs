using MediatR;
using Shared.Events;
using Shared.Events.EventDispatching;
using Shared.Results;

namespace Shared.RequestHandling;

//TODO: Does ApplicationRequestHandler<TRequest, TResponse> class really belong to Shared project?
public abstract class ApplicationRequestHandler<TRequest, TResponse> : IRequestHandler<TRequest, ApplicationResponse<TResponse>>
    where TRequest : IRequest<ApplicationResponse<TResponse>>
{
    protected IMediator Mediator { get; }
    private IEventDispatching _eventDispatcher;

    public ApplicationRequestHandler(
        IMediator mediator,
        IEventDispatching eventDispatcher)
    {
        Mediator = mediator;
        _eventDispatcher = eventDispatcher;
    }

    public abstract Task<ApplicationResponse<TResponse>> Handle(TRequest request, CancellationToken cancellationToken);

    protected void AddEvent(IEvent @event)
        => _eventDispatcher?.AddEvent(@event);
}