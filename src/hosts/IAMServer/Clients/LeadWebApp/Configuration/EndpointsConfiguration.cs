using IAMServer.Clients.LeadWebApp.Features.UsersManagement.Configuration;

namespace IAMServer.Clients.LeadWebApp.Configuration;

public static class EndpointsConfiguration
{
	public static IEndpointRouteBuilder MapEndpoints(this WebApplication app)
		=> app.MapGroup("/api")
			  .MapUserManagementFeaturesEndpoints();
}
