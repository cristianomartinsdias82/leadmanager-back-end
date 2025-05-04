using Domain.Prospecting.Entities;
using Shared.DataPagination;

namespace Application.Core.Contracts.Repository.Prospecting;

public interface ILeadRepository : IRepository<Lead>
{
	Task<PagedList<LeadsFile>> GetLeadsFilesAsync(
		PaginationOptions paginationOptions,
		CancellationToken cancellationToken = default);

	Task AddLeadsFileAsync(
		LeadsFile leadsFile,
		CancellationToken cancellationToken = default);

	Task<LeadsFile?> GetLeadsFileByIdAsync(
		Guid id,
		CancellationToken cancellationToken = default);

	Task RemoveLeadsFilesByIdsAsync(
		IEnumerable<Guid> ids,
		CancellationToken cancellationToken = default);
}