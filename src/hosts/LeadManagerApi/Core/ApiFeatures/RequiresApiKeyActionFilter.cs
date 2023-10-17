using LeadManagerApi.Core.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace LeadManagerApi.Core.ApiFeatures;

public sealed class RequiresApiKeyActionFilter : ActionFilterAttribute
{
    private readonly LeadManagerApiSettings _apiSettings;

    public RequiresApiKeyActionFilter(LeadManagerApiSettings apiSettings)
    {
        _apiSettings = apiSettings;
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (_apiSettings.ApiKeyRequestHeaderRequired)
        {
            if (!context.HttpContext.Request.Headers.TryGetValue(_apiSettings.ApiKeyRequestHeaderName!, out var apiKey) ||
                !string.CompareOrdinal(apiKey, _apiSettings.ApiKeyRequestHeaderValue).Equals(0))
            {
                context.Result = new UnauthorizedResult();
                return;
            }
        }

        base.OnActionExecuting(context);
    }
}