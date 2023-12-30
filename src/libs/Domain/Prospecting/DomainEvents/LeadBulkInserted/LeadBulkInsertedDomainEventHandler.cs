using MediatR;
using Shared.Events.DomainEvents;

namespace Domain.Prospecting.DomainEvents.LeadBulkInserted;

internal sealed class LeadBulkInsertedDomainEventHandler : ApplicationDomainEventHandler<LeadBulkInsertedDomainEvent>
{
    public LeadBulkInsertedDomainEventHandler(
        IMediator mediator) : base(mediator)
    {
    }

    public override async Task Handle(LeadBulkInsertedDomainEvent notification, CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
    }
}
