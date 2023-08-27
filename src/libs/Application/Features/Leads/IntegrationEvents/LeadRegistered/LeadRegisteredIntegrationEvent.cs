using CrossCutting.MessageContracts;
using Shared.Events.IntegrationEvents;

namespace Application.Features.Leads.IntegrationEvents.LeadRegistered;

public sealed record LeadRegisteredIntegrationEvent(LeadData Lead) : IIntegrationEvent;
