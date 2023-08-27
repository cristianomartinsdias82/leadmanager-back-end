using CrossCutting.MessageContracts;

namespace Application.Contracts.Caching;

public interface ICachingManagement
{
    Task AddLeadEntryAsync(LeadData lead, CancellationToken cancellationToken = default);
    Task AddLeadEntriesAsync(List<LeadData> leads, CancellationToken cancellationToken = default);
    Task UpdateLeadEntryAsync(LeadData lead, CancellationToken cancellationToken = default);
    Task RemoveLeadEntryAsync(LeadData lead, CancellationToken cancellationToken = default);
    Task<IEnumerable<LeadData>> GetLeadsAsync(CancellationToken cancellationToken = default);
    Task ClearLeadEntriesAsync(CancellationToken cancellationToken = default);
}