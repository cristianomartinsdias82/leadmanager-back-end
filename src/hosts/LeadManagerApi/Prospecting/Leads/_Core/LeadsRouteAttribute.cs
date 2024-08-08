using LeadManagerApi.Core.ApiFeatures;

namespace LeadManagerApi.Prospecting.Leads.Core;

public sealed class LeadsRouteAttribute : LeadManagerApiRouteAttribute
{
    public LeadsRouteAttribute() : base("leads") { }
}