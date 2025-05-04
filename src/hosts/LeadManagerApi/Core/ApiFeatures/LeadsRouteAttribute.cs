namespace LeadManagerApi.Core.ApiFeatures;

public sealed class LeadsRouteAttribute : LeadManagerApiRouteAttribute
{
    /// <summary>
    /// Base Leads route attribute
    /// </summary>
    /// <param name="routeParams">Omit the leading forward slash for these route params.</param>
    public LeadsRouteAttribute(string? routeParams = null)
        : base($"leads{(!string.IsNullOrWhiteSpace(routeParams) ? "/" + routeParams : string.Empty)}") { }
}