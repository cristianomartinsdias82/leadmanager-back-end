﻿using Application.Core.Contracts.Messaging;
using Application.Prospecting.Leads.Shared;
using MediatR;
using Shared.Events.IntegrationEvents;

namespace Application.Prospecting.Leads.IntegrationEvents.LeadUpdated;

internal sealed class LeadUpdatedIntegrationEventHandler : ApplicationIntegrationEventHandler<LeadUpdatedIntegrationEvent>
{
    private readonly IMessageBusHelper _messageBusHelper;

    public LeadUpdatedIntegrationEventHandler(
        IMediator mediator,
        IMessageBusHelper messageBusHelper) : base(mediator)
    {
        _messageBusHelper = messageBusHelper;
    }

    public override async Task Handle(LeadUpdatedIntegrationEvent notification, CancellationToken cancellationToken)
    => await _messageBusHelper.SendToLeadUpdatedChannelAsync(
                    notification.Lead.MapToMessageContract(),
                    cancellationToken: cancellationToken);
}
