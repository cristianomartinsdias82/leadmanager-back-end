using MediatR;
using Microsoft.Extensions.Logging;
using Shared.Events.DomainEvents;

namespace Core.DomainEvents.LeadRegistered;

internal sealed class LeadRegisteredDomainEventHandler : ApplicationDomainEventHandler<LeadRegisteredDomainEvent>
{
    private readonly ILogger<LeadRegisteredDomainEventHandler> _logger;

    public LeadRegisteredDomainEventHandler(
        IMediator mediator,
        ILogger<LeadRegisteredDomainEventHandler> logger) : base(mediator)
    {
        _logger = logger;
    }

    public override async Task Handle(LeadRegisteredDomainEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("A lead registration domain event has happened: {event} - Data: {notification}", notification.GetType().Name, notification.Lead);

        await Task.CompletedTask;
    }
}
