using MediatR;
using Microsoft.Extensions.Logging;
using Shared.Events.DomainEvents;

namespace Core.DomainEvents.LeadRemoved;

internal sealed class LeadRemovedDomainEventHandler : ApplicationDomainEventHandler<LeadRemovedDomainEvent>
{
    private readonly ILogger<LeadRemovedDomainEventHandler> _logger;

    public LeadRemovedDomainEventHandler(
        IMediator mediator,
        ILogger<LeadRemovedDomainEventHandler> logger) : base(mediator)
    {
        _logger = logger;
    }

    public override async Task Handle(LeadRemovedDomainEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("A lead exclusion domain event has happened: {event} - Data: {notification}", notification.GetType().Name, notification.Lead);

        await Task.CompletedTask;
    }
}
