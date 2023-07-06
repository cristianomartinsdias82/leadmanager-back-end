using MediatR;
using Shared.Results;

namespace Shared.RequestHandling;

public abstract class ApplicationRequestHandler<TRequest, TResponse> : IRequestHandler<TRequest, ApplicationResponse<TResponse>>
    where TRequest : IRequest<ApplicationResponse<TResponse>>
{
    public abstract Task<ApplicationResponse<TResponse>> Handle(TRequest request, CancellationToken cancellationToken);
}