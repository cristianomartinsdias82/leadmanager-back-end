using Infrastructure.Persistence.Mappings;
using Microsoft.EntityFrameworkCore;
using Shared.Results;

namespace LeadManagerApi.ApiFeatures;

public sealed class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(
        RequestDelegate next,
        ILogger<ErrorHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exc)
        {
            var inconsistencies = default(List<Inconsistency>);

            HandleError(exc, ref inconsistencies);
            
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsJsonAsync(
                                    ApplicationResponse<object>.Create(
                                        default!,
                                        "Houve um erro técnico ao processar a solicitação.",
                                        exception: exc.AsExceptionData(),
                                        inconsistencies: inconsistencies?.ToArray() ?? null!));

            inconsistencies?.Clear();
            inconsistencies = null;
        }
    }

    private void HandleError(Exception exc, ref List<Inconsistency>? inconsistencies)
    {
        if (exc is DbUpdateException)
        {
            inconsistencies = new List<Inconsistency>();

            var message = exc.InnerException?.Message ?? string.Empty;
            if (message.Contains(LeadEntityMetadata.CnpjColumnIndexName))
                inconsistencies.Add(new(string.Empty, "Cnpj existente."));

            if (message.Contains(LeadEntityMetadata.RazaoSocialColumnIndexName))
                inconsistencies.Add(new(string.Empty, "Razão Social existente."));

            return;
        }
        
        _logger.LogError(exc, "An error occurred while attempting to process the request.");
    }
}