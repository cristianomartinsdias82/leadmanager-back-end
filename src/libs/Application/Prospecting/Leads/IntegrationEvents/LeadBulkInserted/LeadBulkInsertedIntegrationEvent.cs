using Application.Prospecting.Leads.Shared;
using Shared.Events.IntegrationEvents;

namespace Application.Prospecting.Leads.IntegrationEvents.LeadBulkInserted;

public sealed record LeadBulkInsertedIntegrationEvent(List<LeadDto> Leads) : IIntegrationEvent;
