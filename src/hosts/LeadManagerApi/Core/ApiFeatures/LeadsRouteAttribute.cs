using Microsoft.AspNetCore.Mvc;

namespace LeadManagerApi.Core.ApiFeatures;

public sealed class LeadsRouteAttribute : LeadManagerApiRouteAttribute
{
    public LeadsRouteAttribute() : base("leads") { }
}