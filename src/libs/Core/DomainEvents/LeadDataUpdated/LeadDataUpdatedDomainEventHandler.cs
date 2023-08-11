using MediatR;
using Microsoft.Extensions.Logging;
using Shared.Events.DomainEvents;

namespace Core.DomainEvents.LeadDataUpdated;

internal sealed class LeadDataUpdatedDomainEventHandler : ApplicationDomainEventHandler<LeadDataUpdatedDomainEvent>
{
    private readonly ILogger<LeadDataUpdatedDomainEventHandler> _logger;

    public LeadDataUpdatedDomainEventHandler(
        IMediator mediator,
        ILogger<LeadDataUpdatedDomainEventHandler> logger) : base(mediator)
    {
        _logger = logger;
    }

    public override async Task Handle(LeadDataUpdatedDomainEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("A domain event has happened: {event} - Data: {notification}", notification.GetType().Name, notification);

        await Task.CompletedTask;
    }
}
