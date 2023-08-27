using CrossCutting.MessageContracts;
using Shared.Events.IntegrationEvents;

namespace Application.Features.Leads.IntegrationEvents.LeadUpdated;

public sealed record LeadUpdatedIntegrationEvent(LeadData Lead) : IIntegrationEvent;
