using Application.Features.Leads.Shared;

namespace Application.Contracts.Messaging;

public interface IMessageBusHelper
{
    Task SendToNewlyCreatedLeadsChannelAsync(List<LeadDto> newlyCreatedLeads, CancellationToken cancellationToken = default);
    Task SendToLeadUpdateChannelAsync(LeadDto updatedLead, CancellationToken cancellationToken = default);
    Task SendToLeadRemovedChannelAsync(LeadDto removedLead, CancellationToken cancellationToken = default);
}
