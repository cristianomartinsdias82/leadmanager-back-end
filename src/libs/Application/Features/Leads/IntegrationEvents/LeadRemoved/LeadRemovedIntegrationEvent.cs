using Application.Features.Leads.Shared;
using Shared.Events.IntegrationEvents;

namespace Application.Features.Leads.IntegrationEvents.LeadRemoved;

public sealed record LeadRemovedIntegrationEvent(LeadDto Lead) : IIntegrationEvent;
