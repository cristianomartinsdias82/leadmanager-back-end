using Shared.Events.IntegrationEvents;

namespace Application.Security.OneTimePassword.IntegrationEvents.OneTimePasswordGenerated;

public sealed record OneTimePasswordGeneratedIntegrationEvent(Guid UserId, string Resource, string Code) : IIntegrationEvent;