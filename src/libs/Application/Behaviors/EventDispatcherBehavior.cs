//using MediatR;
//using Shared.Events;

//namespace Application.Behaviors;

//internal sealed class EventDispatcherBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
//    where TRequest : IRequest<TResponse>
//{
//    private readonly IEventDispatching _eventDispatcher;

//    public EventDispatcherBehavior(IEventDispatching eventDispatcher)
//    {
//        _eventDispatcher = eventDispatcher;
//    }

//    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
//    {
//        await _eventDispatcher.DispatchEventsAsync(cancellationToken);

//        return await next();
//    }
//}