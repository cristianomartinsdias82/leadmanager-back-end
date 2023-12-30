using Domain.Prospecting.Entities;
using Shared.Events.IntegrationEvents;

namespace Application.Prospecting.Leads.IntegrationEvents.LeadRemoved;

public sealed record LeadRemovedIntegrationEvent(LeadDto Lead) : IIntegrationEvent;
