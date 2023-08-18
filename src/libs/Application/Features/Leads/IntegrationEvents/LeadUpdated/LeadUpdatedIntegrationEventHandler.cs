using Application.Contracts.Caching;
using MediatR;
using Microsoft.Extensions.Logging;
using Shared.Events.IntegrationEvents;

namespace Application.Features.Leads.IntegrationEvents.LeadUpdated;

internal sealed class LeadUpdatedIntegrationEventHandler : ApplicationIntegrationEventHandler<LeadUpdatedIntegrationEvent>
{
    private readonly ICachingManagement _cachingManager;
    private readonly ILogger<LeadUpdatedIntegrationEventHandler> _logger;

    public LeadUpdatedIntegrationEventHandler(
        IMediator mediator,
        ICachingManagement cachingManager,
        ILogger<LeadUpdatedIntegrationEventHandler> logger) : base(mediator)
    {
        _cachingManager = cachingManager;
        _logger = logger;
    }

    public override async Task Handle(LeadUpdatedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("A lead data update integration event has happened: {event} - Data: {notification}", notification.GetType().Name, notification.Lead);

        await _cachingManager.UpdateLeadEntryAsync(notification.Lead);

        await Task.CompletedTask;
    }
}
