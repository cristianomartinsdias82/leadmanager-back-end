using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shared.Results;
using System.Net.Mime;

namespace LeadManagerApi.ApiFeatures;

[Produces(MediaTypeNames.Application.Json)]
public abstract class LeadManagerController : ControllerBase
{
    protected readonly ISender Mediator;
    private static Dictionary<OperationCodes, int> OperationCode_StatusCode_Map = new Dictionary<OperationCodes, int>
    {
        [OperationCodes.Successful] = StatusCodes.Status200OK,
        [OperationCodes.Error] = StatusCodes.Status500InternalServerError,
        [OperationCodes.ValidationFailure] = StatusCodes.Status400BadRequest,
        [OperationCodes.NotFound] = StatusCodes.Status404NotFound,
        [OperationCodes.ConcurrencyIssue] = StatusCodes.Status409Conflict
    };

    public LeadManagerController(ISender mediator)
    {
        ArgumentNullException.ThrowIfNull(mediator);

        Mediator = mediator;
    }

    protected virtual IActionResult Result<TReturn>(
        ApplicationResponse<TReturn> response,
        Func<HttpContext, ApplicationResponse<TReturn>, int>? onSuccessStatusCodeFactory = default,
        Func<HttpContext, ApplicationResponse<TReturn>, int>? onFailureStatusCodeFactory = default)
            => StatusCode(
                    response.Success
                        ? onSuccessStatusCodeFactory?.Invoke(HttpContext, response) ?? OperationCode_StatusCode_Map[response.OperationCode ?? OperationCodes.Successful]
                        : onFailureStatusCodeFactory?.Invoke(HttpContext, response) ?? OperationCode_StatusCode_Map[response.OperationCode ?? OperationCodes.Error],
                    response);
}