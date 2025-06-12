namespace LeadManagerApi.Core.ApiFeatures;

public sealed class ReportingRouteAttribute : LeadManagerApiRouteAttribute
{
    /// <summary>
    /// Base Leads route attribute
    /// </summary>
    /// <param name="routeParams">Omit the leading forward slash for these route params.</param>
    public ReportingRouteAttribute(string? routeParams = null)
        : base($"reporting{(!string.IsNullOrWhiteSpace(routeParams) ? "/" + routeParams : string.Empty)}") { }
}