using IAMServer.Clients.LeadWebApp.Features.UsersManagement.ListUsers;

namespace IAMServer.Clients.LeadWebApp.Features.UsersManagement.Configuration;

internal static class UsersManagementFeaturesConfiguration
{
	public static IEndpointRouteBuilder MapUserManagementFeaturesEndpoints(this IEndpointRouteBuilder routeGroupBuilder)
		=> routeGroupBuilder
			.MapGroup("users")
			.MapListUsersFeatureEndpoint();
}
