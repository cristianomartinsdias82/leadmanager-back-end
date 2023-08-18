using MediatR;
using Microsoft.Extensions.Logging;
using Shared.Events.DomainEvents;

namespace Core.DomainEvents.LeadUpdated;

internal sealed class LeadUpdatedDomainEventHandler : ApplicationDomainEventHandler<LeadUpdatedDomainEvent>
{
    private readonly ILogger<LeadUpdatedDomainEventHandler> _logger;

    public LeadUpdatedDomainEventHandler(
        IMediator mediator,
        ILogger<LeadUpdatedDomainEventHandler> logger) : base(mediator)
    {
        _logger = logger;
    }

    public override async Task Handle(LeadUpdatedDomainEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("A lead data update domain event has happened: {event} - Data: {notification}", notification.GetType().Name, notification.Lead);

        await Task.CompletedTask;
    }
}
