using MediatR;
using Shared.DataQuerying;
using Shared.Results;

namespace Application.Security.UsersManagement.Queries.ListUsers;

public sealed record ListUsersQueryRequest()
	: IRequest<ApplicationResponse<PagedList<UserDto>>>;