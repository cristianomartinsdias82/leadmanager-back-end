using Microsoft.AspNetCore.Mvc;

namespace LeadManagerApi.ApiFeatures;

public sealed class LeadsRouteAttribute : LeadManagerApiRouteAttribute
{
    public LeadsRouteAttribute() : base("leads") { }
}