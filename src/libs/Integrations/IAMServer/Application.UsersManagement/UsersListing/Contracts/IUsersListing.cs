using Application.UsersManagement.UsersListing.Models;

namespace Application.UsersManagement.UsersListing.Contracts;

public interface IUsersListing
{
	Task<List<User>> ListUsersAsync(CancellationToken cancellationToken = default);
}
