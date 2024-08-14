namespace LeadManagerApi.Core.ApiFeatures;

internal static class HttpContextExtensions
{
    public static string GetBaseUrl(this HttpContext httpContext)
    {
        if (httpContext is null)
            return default!;

        var request = httpContext.Request;
        var uriBuilder = new UriBuilder
        {
            Scheme = request.Scheme,
            Host = request.Host.Host,
            Port = request.Host.Port ?? (request.Scheme == "https" ? 443 : 80)
        };

        return uriBuilder.Uri.AbsoluteUri.TrimEnd('/');
    }
}
