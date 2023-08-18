using Application.Features.Leads.Shared;
using Shared.Events.IntegrationEvents;

namespace Application.Features.Leads.IntegrationEvents.LeadBulkInserted;

public sealed record LeadBulkInsertedIntegrationEvent(List<LeadDto> Leads) : IIntegrationEvent;
