using Application.UsersManagement.UsersListing.Models;
using CrossCutting.Caching;
using IAMServer.ServiceClient;
using MediatR;
using Shared.DataQuerying;
using Shared.RequestHandling;
using Shared.Results;

namespace Application.Security.UsersManagement.Queries.ListUsers;

internal sealed class ListUsersQueryRequestHandler : ApplicationRequestHandler<ListUsersQueryRequest, PagedList<UserDto>>
{
	private readonly IIAMServerUsersManagementServiceClient _IAMServerUsersManagementServiceClient;
	private readonly ICacheProvider _cacheProvider;

	public ListUsersQueryRequestHandler(
		IIAMServerUsersManagementServiceClient IAMServerUsersManagementServiceClient,
        ICacheProvider cacheProvider,
		IMediator mediator)
         : base(mediator, default!)
    {
		_IAMServerUsersManagementServiceClient = IAMServerUsersManagementServiceClient;
        _cacheProvider = cacheProvider;
	}

    public async override Task<ApplicationResponse<PagedList<UserDto>>> Handle(
        ListUsersQueryRequest request,
        CancellationToken cancellationToken = default)
    {
        const string UsersListCacheKey = "iamservers_users";

        var usersList = await _cacheProvider.GetOrCreateAsync(
            UsersListCacheKey,
            async ct => {
                var usersList = await _IAMServerUsersManagementServiceClient.ListUsersAsync(ct);

                return usersList.Select(ListUsersMapper.Map);
            },
            [UsersListCacheKey]);

        return ApplicationResponse<PagedList<UserDto>>
                .Create(PagedList<UserDto>.Paginate(
                                            usersList,
                                            PaginationOptions.SinglePage()));
    }
}
