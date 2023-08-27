using CrossCutting.MessageContracts;
using Shared.Events.IntegrationEvents;

namespace Application.Features.Leads.IntegrationEvents.LeadBulkInserted;

public sealed record LeadBulkInsertedIntegrationEvent(List<LeadData> Leads) : IIntegrationEvent;
