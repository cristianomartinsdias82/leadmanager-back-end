using Serilog.Context;

namespace LeadManagerApi.Core.ApiFeatures;

public class RequestLogContextMiddleware
{
    private readonly RequestDelegate _next;
    private const string CorrelationId = "CorrelationId";

    public RequestLogContextMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public Task InvokeAsync(HttpContext httpContext)
    {
        using (LogContext.PushProperty("LeadManager-Api-Key", httpContext.Request.Headers["LeadManager-Api-Key"]))
        using (LogContext.PushProperty(CorrelationId, httpContext.TraceIdentifier))
            return _next(httpContext);
    }
}
