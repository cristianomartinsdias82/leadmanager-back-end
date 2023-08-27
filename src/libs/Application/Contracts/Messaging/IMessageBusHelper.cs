using CrossCutting.MessageContracts;

namespace Application.Contracts.Messaging;

public interface IMessageBusHelper
{
    ValueTask SendToNewlyCreatedLeadsChannelAsync(
        List<LeadData> newlyCreatedLeads,
        CancellationToken cancellationToken = default);

    ValueTask SendToLeadUpdatedChannelAsync(
        LeadData updatedLead,
        CancellationToken cancellationToken = default);

    ValueTask SendToLeadRemovedChannelAsync(
        LeadData removedLead,
        CancellationToken cancellationToken = default);
}
