using IAMServer.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static IAMServer.Clients.LeadWebApp.Security.LeadManagerAppConstants;

namespace IAMServer.Clients.LeadWebApp.Features.UsersManagement.ListUsers;

internal static class ListsUsersFeature
{
	internal sealed record ListUsersDto(Guid Id, string? Name, string? Email);
	internal sealed record ListUsersResponse(List<ListUsersDto> Users);

	public static IEndpointRouteBuilder MapListUsersFeatureEndpoint(this IEndpointRouteBuilder routeBuilder)
	{
		routeBuilder.MapGet(string.Empty, ListUsersHandler.Handle)
			.WithName("ListUsers")
			.WithDescription("Lists the users.")
			.WithTags("list", "retrieve", "get", "users")
			.WithSummary("Lists the users.")
			.RequireAuthorization(Policies.LeadManagerAdministrativeTasksPolicy);

		return routeBuilder;
	}

	private static class ListUsersMapper
	{
		public static ListUsersDto Map(ApplicationUser user)
			=> new(user.Id, user.UserName, user.Email);
	}

	private static class ListUsersHandler
	{
		public static Delegate Handle
			=> (ClaimsPrincipal? user,
				[FromServices] UserManager<ApplicationUser> userManager,
				CancellationToken cancellationToken) =>
			{
				var users = userManager.Users.ToList();

				return TypedResults.Ok(users.Select(ListUsersMapper.Map));
			};
	}
}