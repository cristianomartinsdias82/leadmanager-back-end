using Application.Contracts.Caching;
using MediatR;
using Microsoft.Extensions.Logging;
using Shared.Events.IntegrationEvents;

namespace Application.Features.Leads.IntegrationEvents.LeadRemoved;

internal sealed class LeadRemovedIntegrationEventHandler : ApplicationIntegrationEventHandler<LeadRemovedIntegrationEvent>
{
    private readonly ICachingManagement _cachingManager;
    private readonly ILogger<LeadRemovedIntegrationEventHandler> _logger;

    public LeadRemovedIntegrationEventHandler(
        IMediator mediator,
        ICachingManagement cachingManager,
        ILogger<LeadRemovedIntegrationEventHandler> logger) : base(mediator)
    {
        _cachingManager = cachingManager;
        _logger = logger;
    }

    public override async Task Handle(LeadRemovedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("A lead exclusion integration event has happened: {event} - Data: {notification}", notification.GetType().Name, notification.Lead);

        await _cachingManager.RemoveLeadEntryAsync(notification.Lead);

        await Task.CompletedTask;
    }
}
