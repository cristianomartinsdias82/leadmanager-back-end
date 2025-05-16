using Domain.Prospecting.Entities;
using Shared.DataQuerying;

namespace Application.Core.Contracts.Repository.Caching;

public interface ICachingLeadRepository
{
	Task<PagedList<LeadDto>> GetAsDtoAsync(
		PaginationOptions? paginationOptions = default,
		QueryOptions? queryOptions = default,
		CancellationToken cancellationToken = default);

	Task ClearAsync(CancellationToken cancellationToken = default);
}
