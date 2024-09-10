using LeadManagerApi.Core.ApiFeatures;

namespace LeadManagerApi.Security.Auditing.Core;

public sealed class AuditingRouteAttribute : LeadManagerApiRouteAttribute
{
    public AuditingRouteAttribute() : base("auditing") { }
}