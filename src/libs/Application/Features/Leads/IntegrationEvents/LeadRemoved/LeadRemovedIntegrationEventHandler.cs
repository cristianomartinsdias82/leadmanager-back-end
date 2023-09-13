using Application.Contracts.Messaging;
using Application.Features.Leads.Shared;
using MediatR;
using Shared.Events.IntegrationEvents;

namespace Application.Features.Leads.IntegrationEvents.LeadRemoved;

internal sealed class LeadRemovedIntegrationEventHandler : ApplicationIntegrationEventHandler<LeadRemovedIntegrationEvent>
{
    private readonly IMessageBusHelper _messageBusHelper;

    public LeadRemovedIntegrationEventHandler(
        IMediator mediator,
        IMessageBusHelper messageBusHelper) : base(mediator)
    {
        _messageBusHelper = messageBusHelper;
    }

    public override async Task Handle(LeadRemovedIntegrationEvent notification, CancellationToken cancellationToken)
    => await _messageBusHelper.SendToLeadRemovedChannelAsync(
                    notification.Lead.MapToMessageContract(),
                    cancellationToken: cancellationToken);
}
