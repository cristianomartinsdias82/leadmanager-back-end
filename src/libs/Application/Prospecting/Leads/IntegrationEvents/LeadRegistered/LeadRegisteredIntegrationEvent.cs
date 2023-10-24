using Application.Prospecting.Leads.Shared;
using Shared.Events.IntegrationEvents;

namespace Application.Prospecting.Leads.IntegrationEvents.LeadRegistered;

public sealed record LeadRegisteredIntegrationEvent(LeadDto Lead) : IIntegrationEvent;
