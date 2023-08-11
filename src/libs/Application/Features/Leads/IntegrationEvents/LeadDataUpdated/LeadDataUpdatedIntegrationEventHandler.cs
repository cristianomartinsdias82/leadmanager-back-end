using MediatR;
using Microsoft.Extensions.Logging;
using Shared.Events.IntegrationEvents;

namespace Application.Features.Leads.IntegrationEvents.LeadDataUpdated;

internal sealed class LeadDataUpdatedIntegrationEventHandler : ApplicationIntegrationEventHandler<LeadDataUpdatedIntegrationEvent>
{
    private readonly ILogger<LeadDataUpdatedIntegrationEventHandler> _logger;

    public LeadDataUpdatedIntegrationEventHandler(
        IMediator mediator,
        ILogger<LeadDataUpdatedIntegrationEventHandler> logger) : base(mediator)
    {
        _logger = logger;
    }

    public override async Task Handle(LeadDataUpdatedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("An integration event has happened: {event} - Data: {notification}", notification.GetType().Name, notification);

        await Task.CompletedTask;
    }
}
