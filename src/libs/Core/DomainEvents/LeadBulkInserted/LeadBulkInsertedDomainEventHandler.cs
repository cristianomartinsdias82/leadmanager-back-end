using MediatR;
using Microsoft.Extensions.Logging;
using Shared.Events.DomainEvents;

namespace Core.DomainEvents.LeadBulkInserted;

internal sealed class LeadBulkInsertedDomainEventHandler : ApplicationDomainEventHandler<LeadBulkInsertedDomainEvent>
{
    private readonly ILogger<LeadBulkInsertedDomainEventHandler> _logger;

    public LeadBulkInsertedDomainEventHandler(
        IMediator mediator,
        ILogger<LeadBulkInsertedDomainEventHandler> logger) : base(mediator)
    {
        _logger = logger;
    }

    public override async Task Handle(LeadBulkInsertedDomainEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("A lead bulk insert domain event has happened: {event} - Data: {notification}", notification.GetType().Name, notification.Leads);

        await Task.CompletedTask;
    }
}
