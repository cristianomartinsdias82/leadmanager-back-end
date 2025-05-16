namespace Application.Security.UsersManagement.Queries.ListUsers;

public sealed record UserDto(
	string Id,
	string Name,
	string Email);