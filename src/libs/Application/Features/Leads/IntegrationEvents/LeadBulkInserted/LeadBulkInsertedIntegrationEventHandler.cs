using Application.Contracts.Caching;
using Application.Contracts.Messaging;
using MediatR;
using Shared.Events.IntegrationEvents;

namespace Application.Features.Leads.IntegrationEvents.LeadBulkInserted;

internal sealed class LeadBulkInsertedIntegrationEventHandler : ApplicationIntegrationEventHandler<LeadBulkInsertedIntegrationEvent>
{
    private readonly ICachingManagement _cachingManager;
    private readonly IMessageBusHelper _messageBusHelper;

    public LeadBulkInsertedIntegrationEventHandler(
        IMediator mediator,
        ICachingManagement cachingManager,
        IMessageBusHelper messageBusHelper) : base(mediator)
    {
        _cachingManager = cachingManager;
        _messageBusHelper = messageBusHelper;
    }

    public override async Task Handle(LeadBulkInsertedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        await _messageBusHelper.SendToNewlyCreatedLeadsChannelAsync(
                    notification.Leads,
                    cancellationToken: cancellationToken);

        await _cachingManager.AddLeadEntriesAsync(notification.Leads, cancellationToken);
    }
}