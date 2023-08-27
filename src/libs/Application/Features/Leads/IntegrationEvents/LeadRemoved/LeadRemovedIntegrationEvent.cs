using CrossCutting.MessageContracts;
using Shared.Events.IntegrationEvents;

namespace Application.Features.Leads.IntegrationEvents.LeadRemoved;

public sealed record LeadRemovedIntegrationEvent(LeadData Lead) : IIntegrationEvent;
