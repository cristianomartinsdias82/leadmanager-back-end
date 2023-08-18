using Application.Contracts.Caching;
using MediatR;
using Microsoft.Extensions.Logging;
using Shared.Events.IntegrationEvents;

namespace Application.Features.Leads.IntegrationEvents.LeadRegistered;

internal sealed class LeadRegisteredIntegrationEventHandler : ApplicationIntegrationEventHandler<LeadRegisteredIntegrationEvent>
{
    private readonly ICachingManagement _cachingManager;
    private readonly ILogger<LeadRegisteredIntegrationEventHandler> _logger;

    public LeadRegisteredIntegrationEventHandler(
        IMediator mediator,
        ICachingManagement cachingManager,
        ILogger<LeadRegisteredIntegrationEventHandler> logger) : base(mediator)
    {
        _cachingManager = cachingManager;
        _logger = logger;
    }

    public override async Task Handle(LeadRegisteredIntegrationEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("A lead registration integration event has happened: {event} - Data: {notification}", notification.GetType().Name, notification.Lead);

        await _cachingManager.AddLeadEntryAsync(notification.Lead);

        await Task.CompletedTask;
    }
}
