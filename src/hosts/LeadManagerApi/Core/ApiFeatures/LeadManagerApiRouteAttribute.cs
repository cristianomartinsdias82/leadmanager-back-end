using Microsoft.AspNetCore.Mvc;

namespace LeadManagerApi.Core.ApiFeatures;

public class LeadManagerApiRouteAttribute : RouteAttribute
{
    public const string ApiRoutePrefix = "/api";

    public LeadManagerApiRouteAttribute(string template) : base($"{ApiRoutePrefix}/{template}") { }
}