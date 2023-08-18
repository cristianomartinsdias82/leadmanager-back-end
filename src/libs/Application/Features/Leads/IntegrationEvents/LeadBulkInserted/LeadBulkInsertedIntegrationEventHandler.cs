using Application.Contracts.Caching;
using MediatR;
using Microsoft.Extensions.Logging;
using Shared.Events.IntegrationEvents;

namespace Application.Features.Leads.IntegrationEvents.LeadBulkInserted;

internal sealed class LeadBulkInsertedIntegrationEventHandler : ApplicationIntegrationEventHandler<LeadBulkInsertedIntegrationEvent>
{
    private readonly ICachingManagement _cachingManager;
    private readonly ILogger<LeadBulkInsertedIntegrationEventHandler> _logger;

    public LeadBulkInsertedIntegrationEventHandler(
        IMediator mediator,
        ICachingManagement cachingManager,
        ILogger<LeadBulkInsertedIntegrationEventHandler> logger) : base(mediator)
    {
        _cachingManager = cachingManager;
        _logger = logger;
    }

    public override async Task Handle(LeadBulkInsertedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("A lead bulk insert integration event has happened: {event} - Data: {notification}", notification.GetType().Name, notification.Leads);

        await _cachingManager.AddLeadEntriesAsync(notification.Leads);

        await Task.CompletedTask;
    }
}