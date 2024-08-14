using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Results;
using System.Net.Mime;
using static Application.Security.LeadManagerSecurityConfiguration;

namespace LeadManagerApi.Core.ApiFeatures;

[Produces(MediaTypeNames.Application.Json)]
[Authorize(Policy = Policies.LeadManagerDefaultPolicy)]
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
        Func<HttpContext, ApplicationResponse<TReturn>, int>? onFailureStatusCodeFactory = default,
        (string Endpoint, Guid DataId)? routeData = default)
    {
        if (response.Success)
        {
            if (onSuccessStatusCodeFactory is not null)
            {
                var statusCode = onSuccessStatusCodeFactory(HttpContext, response);
                return statusCode switch
                {
                    StatusCodes.Status201Created => routeData is not null ?
                                                    Created(new Uri($"{HttpContext.GetBaseUrl()}{LeadManagerApiRouteAttribute.ApiRoutePrefix}/{routeData.Value.Endpoint}/{routeData.Value.DataId}"), new { id = routeData.Value.DataId })
                                                    :
                                                    Ok(response),
                    StatusCodes.Status204NoContent => NoContent(),
                    _ => Ok(response)
                };
            }

            return Ok(response);
        }

        return StatusCode(
                onFailureStatusCodeFactory?.Invoke(HttpContext, response) ?? OperationCode_StatusCode_Map[response.OperationCode ?? OperationCodes.Error],
                response);
    }
}