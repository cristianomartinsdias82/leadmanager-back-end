using Domain.Prospecting.Entities;
using Shared.Events.IntegrationEvents;

namespace Application.Prospecting.Leads.IntegrationEvents.LeadUpdated;

public sealed record LeadUpdatedIntegrationEvent(LeadDto Lead) : IIntegrationEvent;
