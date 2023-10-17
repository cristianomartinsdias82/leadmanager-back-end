using Microsoft.AspNetCore.Mvc;

namespace LeadManagerApi.Core.ApiFeatures;

public class LeadManagerApiRouteAttribute : RouteAttribute
{
    protected const string _apiRoutePrefix = "/api";

    public LeadManagerApiRouteAttribute(string template) : base($"{_apiRoutePrefix}/{template}") { }
}