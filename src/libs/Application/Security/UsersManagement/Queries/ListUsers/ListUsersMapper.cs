using Application.UsersManagement.UsersListing.Models;

namespace Application.Security.UsersManagement.Queries.ListUsers;

internal static class ListUsersMapper
{
	public static UserDto Map(this User user)
		=> new(user.Id, user.Name, user.Email);
}