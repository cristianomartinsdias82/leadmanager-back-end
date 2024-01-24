using MediatR;
using Microsoft.Extensions.Logging;
using Shared.Results;
using CrossCutting.Logging.Extensions;

namespace Application.Core.Behaviors;

internal sealed class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, ApplicationResponse<TResponse>>
    where TRequest : IRequest<ApplicationResponse<TResponse>>    
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<ApplicationResponse<TResponse>> Handle(TRequest request, RequestHandlerDelegate<ApplicationResponse<TResponse>> next, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;

        _logger.LogInformation("Processing request {RequestName}...", requestName);

        var response = await next();

        if (response.Success)
            _logger.LogInformation("Request {RequestName} processed successfully.", requestName);
        else
            _logger.LogErrorWithPushProperty("Error", response.Exception, "Completed request {RequestName} with error", requestName);

        return response;
    }
}
