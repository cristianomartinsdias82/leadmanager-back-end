using Application.Contracts.Messaging;
using MediatR;
using Shared.Events.IntegrationEvents;

namespace Application.Features.Leads.IntegrationEvents.LeadBulkInserted;

internal sealed class LeadBulkInsertedIntegrationEventHandler : ApplicationIntegrationEventHandler<LeadBulkInsertedIntegrationEvent>
{
    private readonly IMessageBusHelper _messageBusHelper;

    public LeadBulkInsertedIntegrationEventHandler(
        IMediator mediator,
        IMessageBusHelper messageBusHelper) : base(mediator)
    {
        _messageBusHelper = messageBusHelper;
    }

    public override async Task Handle(LeadBulkInsertedIntegrationEvent notification, CancellationToken cancellationToken)
        => await _messageBusHelper.SendToNewlyCreatedLeadsChannelAsync(
                    notification.Leads,
                    cancellationToken: cancellationToken);
}