using Application.Prospecting.Leads.IntegrationEvents.LeadRegistered;
using CrossCutting.Monitoring.LeadManagerApi.CustomMetrics;
using MediatR;
using Shared.Events.IntegrationEvents;

internal sealed class IncrementLeadRegisteredCounterEventHandler : ApplicationIntegrationEventHandler<LeadRegisteredIntegrationEvent>
{
    private readonly ILeadManagerApiMetricsCollection _metricsCollector;

    public IncrementLeadRegisteredCounterEventHandler(
        IMediator mediator,
        ILeadManagerApiMetricsCollection metricsCollector) : base(mediator)
    {
        _metricsCollector = metricsCollector;
    }

    public override Task Handle(LeadRegisteredIntegrationEvent notification, CancellationToken cancellationToken)
        => _metricsCollector.IncrementRegisteredLeadsCounter();
}