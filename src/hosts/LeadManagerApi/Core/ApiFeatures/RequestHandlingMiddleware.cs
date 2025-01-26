using Domain.Prospecting.Exceptions;
using Infrastructure.Persistence.Mappings;
using Microsoft.EntityFrameworkCore;
using Shared.ApplicationOperationRules;
using Shared.Results;

namespace LeadManagerApi.Core.ApiFeatures;

public sealed class RequestHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestHandlingMiddleware> _logger;

    public RequestHandlingMiddleware(
        RequestDelegate next,
        ILogger<RequestHandlingMiddleware> logger)
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

    private void HandleError(
        Exception exc,
        ref List<Inconsistency>? inconsistencies)
    {
        inconsistencies = new List<Inconsistency>();

        if (exc is DbUpdateException)
        {
            var message = exc.InnerException?.Message ?? string.Empty;
            if (message.Contains(LeadEntityMetadata.CnpjColumnIndexName))
                inconsistencies.Add(new(string.Empty, "Cnpj existente."));

            if (message.Contains(LeadEntityMetadata.RazaoSocialColumnIndexName))
                inconsistencies.Add(new(string.Empty, "Razão Social existente."));

            _logger.LogInformation(exc, "Db update exception: {Message}", message);

            return;
        }

        if (exc is BusinessException)
        {
            inconsistencies.Add(new(string.Empty, exc.Message));

            _logger.LogInformation(exc, "Business exception: {Message}", exc.Message);

            return;
        }

        if (exc is ApplicationOperatingRuleException appOperRuleExc)
        {
            inconsistencies.AddRange([..appOperRuleExc.RuleViolations]);

			_logger.LogWarning(
                exc,
                "One or more application operating rules have been violated while attempting to perform the request. {@violatedApplicationOperatingRules}",
                appOperRuleExc.RuleViolations);
        }
    }
}