using Application.Features.Leads.Shared;
using Shared.Events.IntegrationEvents;

namespace Application.Features.Leads.IntegrationEvents.LeadRegistered;

public sealed record LeadRegisteredIntegrationEvent(LeadDto Lead) : IIntegrationEvent;
