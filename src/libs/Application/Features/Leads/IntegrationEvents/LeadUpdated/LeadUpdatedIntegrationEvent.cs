using Application.Features.Leads.Shared;
using Shared.Events.IntegrationEvents;

namespace Application.Features.Leads.IntegrationEvents.LeadUpdated;

public sealed record LeadUpdatedIntegrationEvent(LeadDto Lead) : IIntegrationEvent;
