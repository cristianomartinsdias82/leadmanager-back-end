using Application.Prospecting.Leads.Shared;
using Shared.DataPagination;

namespace Application.Core.Contracts.Caching;

public interface ICachingManagement
{
    Task<PagedList<LeadDto>> GetLeadsAsync(PaginationOptions paginationOptions, CancellationToken cancellationToken = default);
    Task AddLeadEntryAsync(LeadDto lead, CancellationToken cancellationToken = default);
    Task AddLeadEntriesAsync(IList<LeadDto> leads, CancellationToken cancellationToken = default);
    Task UpdateLeadEntryAsync(LeadDto lead, CancellationToken cancellationToken = default);
    Task RemoveLeadEntryAsync(LeadDto lead, CancellationToken cancellationToken = default);
    Task ClearLeadEntriesAsync(CancellationToken cancellationToken = default);
}