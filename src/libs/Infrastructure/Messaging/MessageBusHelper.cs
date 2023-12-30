using Application.Core.Contracts.Messaging;
using CrossCutting.MessageContracts;
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

    public ValueTask SendToLeadRemovedChannelAsync(LeadData removedLead, CancellationToken cancellationToken = default)
        => _messageDispatcher.SendToTopicAsync(
            _messageChannelSettings.RemovedLeadChannel.TopicName,
            _messageChannelSettings.RemovedLeadChannel.RoutingKey,
            removedLead,
            cancellationToken);

    public ValueTask SendToLeadUpdatedChannelAsync(LeadData updatedLead, CancellationToken cancellationToken = default)
        => _messageDispatcher.SendToTopicAsync(
            _messageChannelSettings.UpdatedLeadChannel.TopicName,
            _messageChannelSettings.UpdatedLeadChannel.RoutingKey,
            updatedLead,
            cancellationToken);

    public ValueTask SendToNewlyCreatedLeadsChannelAsync(List<LeadData> newlyCreatedLeads, CancellationToken cancellationToken = default)
        =>_messageDispatcher.SendToTopicAsync(
            _messageChannelSettings.NewlyRegisteredLeadsChannel.TopicName,
            _messageChannelSettings.NewlyRegisteredLeadsChannel.RoutingKey,
            newlyCreatedLeads,
            cancellationToken);
}
