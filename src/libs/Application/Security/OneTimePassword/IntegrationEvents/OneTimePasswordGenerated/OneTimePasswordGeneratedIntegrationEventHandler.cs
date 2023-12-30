using Application.Security.OneTimePassword.IntegrationEvents.OneTimePasswordGenerated;
using CrossCutting.EndUserCommunication.Sms;
using MediatR;
using Shared.Events.IntegrationEvents;

namespace Application.Prospecting.Leads.IntegrationEvents.LeadRegistered;

internal sealed class OneTimePasswordGeneratedIntegrationEventHandler : ApplicationIntegrationEventHandler<OneTimePasswordGeneratedIntegrationEvent>
{
    private readonly ISmsDeliveryService _smsDeliveryService;

    public OneTimePasswordGeneratedIntegrationEventHandler(
        IMediator mediator,
        ISmsDeliveryService smsDeliveryService) : base(mediator)
    {
        _smsDeliveryService = smsDeliveryService;
    }

    public override async Task Handle(OneTimePasswordGeneratedIntegrationEvent notification, CancellationToken cancellationToken)
        => await _smsDeliveryService.SendAsync($"Resource: {notification.Resource} - User id: {notification.UserId} - Code: {notification.Code}",
                                               cancellationToken: cancellationToken);
}
