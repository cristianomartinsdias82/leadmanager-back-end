using Application.Contracts.Messaging;
using Application.Features.Leads.Shared;
using CrossCutting.Messaging;

namespace Infrastructure.Messaging;

public sealed class MessageBusHelper : IMessageBusHelper
{
    private readonly IMessageDispatching _messageDispatcher;
    private readonly MessageChannelSettings _messageChannelSettings;

    public MessageBusHelper(
        IMessageDispatching messageDispatcher,
        MessageChannelSettings messageChannelsSettings)
    {
        _messageDispatcher = messageDispatcher;
        _messageChannelSettings = messageChannelsSettings;
    }

    public Task SendToLeadRemovedChannelAsync(LeadDto removedLead, CancellationToken cancellationToken = default)
        => _messageDispatcher.SendToTopicAsync(
            _messageChannelSettings.RemovedLeadChannel.TopicName,
            removedLead,
            cancellationToken);

    public Task SendToLeadUpdatedChannelAsync(LeadDto updatedLead, CancellationToken cancellationToken = default)
        => _messageDispatcher.SendToTopicAsync(
            _messageChannelSettings.UpdatedLeadChannel.TopicName,
            updatedLead,
            cancellationToken);

    public Task SendToNewlyCreatedLeadsChannelAsync(List<LeadDto> newlyCreatedLeads, CancellationToken cancellationToken = default)
        =>_messageDispatcher.SendToTopicAsync(
            _messageChannelSettings.NewlyRegisteredLeadsChannel.TopicName,
            newlyCreatedLeads,
            cancellationToken);
}
