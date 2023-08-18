using Application.Features.Leads.Shared;

namespace Application.Contracts.Caching;

public interface ICachingManagement
{
    Task AddLeadEntryAsync(LeadDto lead, CancellationToken cancellationToken = default);
    Task AddLeadEntriesAsync(List<LeadDto> leads, CancellationToken cancellationToken = default);
    Task UpdateLeadEntryAsync(LeadDto lead, CancellationToken cancellationToken = default);
    Task RemoveLeadEntryAsync(LeadDto lead, CancellationToken cancellationToken = default);
    Task<IEnumerable<LeadDto>> GetLeadsAsync(CancellationToken cancellationToken = default);
    Task ClearLeadEntriesAsync(CancellationToken cancellationToken = default);
}