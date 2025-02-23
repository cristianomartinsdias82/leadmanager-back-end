using Domain.Prospecting.Exceptions;
using Infrastructure.Persistence.Mappings;
using Microsoft.EntityFrameworkCore;
using Shared.ApplicationOperationRules;
using Shared.Diagnostics;
using Shared.Results;
using System.Diagnostics;

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

            var errorStatusCode = StatusCodes.Status500InternalServerError;
			HandleError(exc, ref inconsistencies, ref errorStatusCode);

			context.Response.StatusCode = errorStatusCode;
            await context.Response.WriteAsJsonAsync(
                                    ApplicationResponse<object>.Create(
                                        data: default!,
                                        message: "Houve um erro técnico ao processar a solicitação.",
                                        exception: exc.AsExceptionData(),
                                        inconsistencies: inconsistencies?.ToArray() ?? null!));

            inconsistencies?.Clear();
            inconsistencies = null;
		}
    }

	private void HandleError(
		Exception exc,
		ref List<Inconsistency>? inconsistencies,
        ref int statusCode)
	{
		inconsistencies = new List<Inconsistency>();

        switch (exc)
        {
            case DbUpdateException dbExc:

				var message = dbExc.InnerException?.Message ?? string.Empty;
				if (message.Contains(LeadEntityMetadata.CnpjColumnIndexName))
					inconsistencies.Add(new(string.Empty, "Cnpj existente."));

				if (message.Contains(LeadEntityMetadata.RazaoSocialColumnIndexName))
					inconsistencies.Add(new(string.Empty, "Razão Social existente."));

				_logger.LogInformation(dbExc, "Db update exception: {Message}", message);

                statusCode = StatusCodes.Status500InternalServerError;

				break;

			case BusinessException bsExc:
				inconsistencies.Add(new(string.Empty, exc.Message));

				_logger.LogInformation(bsExc, "Business exception: {Message}", bsExc.Message);

				statusCode = StatusCodes.Status422UnprocessableEntity;

				break;

            case ApplicationOperatingRuleException oprExc:

				if (oprExc.RuleViolations?.Any() ?? false)
					inconsistencies.AddRange([..oprExc.RuleViolations!]);

				_logger.LogWarning(
					oprExc,
					"One or more application operating rules have been violated while attempting to perform the request. {@violatedApplicationOperatingRules}",
					oprExc.RuleViolations);

                statusCode = StatusCodes.Status400BadRequest;

				break;

            default:

				//Telemetry (Exception Span event)
				DiagnosticsDataCollector
					.WithActivity(Activity.Current)
					.WithException(exc, TimeProvider.System.GetUtcNow())
					.PushData();

				statusCode = StatusCodes.Status500InternalServerError;
				_logger.LogError(
					exc,
					"Error while processing the request: {message}. See stack trace for further details.", exc.Message);

				break;
		}
	}
}