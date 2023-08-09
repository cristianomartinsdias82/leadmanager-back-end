using MediatR;

namespace Application.Features.Leads.IntegrationEvents.LeadDataUpdated;

internal sealed class LeadDataUpdatedIntegrationEventHandler : INotificationHandler<LeadDataUpdatedIntegrationEvent>
{
    private readonly IMediator _mediator;

    public LeadDataUpdatedIntegrationEventHandler(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task Handle(LeadDataUpdatedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
    }
}
