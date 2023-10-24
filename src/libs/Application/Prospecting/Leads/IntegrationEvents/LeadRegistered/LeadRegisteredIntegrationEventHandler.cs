using Application.Core.Contracts.Messaging;
using Application.Prospecting.Leads.Shared;
using CrossCutting.MessageContracts;
using MediatR;
using Shared.Events.IntegrationEvents;

namespace Application.Prospecting.Leads.IntegrationEvents.LeadRegistered;

internal sealed class LeadRegisteredIntegrationEventHandler : ApplicationIntegrationEventHandler<LeadRegisteredIntegrationEvent>
{
    private readonly IMessageBusHelper _messageBusHelper;

    public LeadRegisteredIntegrationEventHandler(
        IMediator mediator,
        IMessageBusHelper messageBusHelper) : base(mediator)
    {
        _messageBusHelper = messageBusHelper;
    }

    public override async Task Handle(LeadRegisteredIntegrationEvent notification, CancellationToken cancellationToken)
        => await _messageBusHelper.SendToNewlyCreatedLeadsChannelAsync(
                    new List<LeadData> { notification.Lead.MapToMessageContract() },
                    cancellationToken: cancellationToken);
}
