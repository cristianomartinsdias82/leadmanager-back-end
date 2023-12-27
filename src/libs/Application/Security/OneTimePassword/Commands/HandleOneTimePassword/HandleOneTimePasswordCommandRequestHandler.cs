using Application.Core.Contracts.Repository.Caching;
using Application.Core.Contracts.Repository.Security;
using Application.Security.OneTimePassword.Commands.GenerateOneTimePassword;
using CrossCutting.EndUserCommunication.Sms;
using MediatR;
using Shared.RequestHandling;
using Shared.Results;

namespace Application.Security.OneTimePassword.Commands.HandleOneTimePassword;

public sealed class HandleOneTimePasswordCommandRequestHandler : ApplicationRequestHandler<HandleOneTimePasswordCommandRequest, HandleOneTimePasswordCommandResponse>
{
    private readonly IOneTimePasswordRepository _oneTimePasswordRepository;
    private readonly OneTimePasswordCachingPolicy _oneTimePasswordCachingPolicy;
    private readonly ISmsDeliveryService _smsDeliveryService;

    public HandleOneTimePasswordCommandRequestHandler(
        OneTimePasswordCachingPolicy oneTimePasswordCachingPolicy,
        IMediator mediator,
        IOneTimePasswordRepository oneTimePasswordRepository,
        ISmsDeliveryService smsDeliveryService) : base(mediator, default!)
    {
        _oneTimePasswordRepository = oneTimePasswordRepository;
        _oneTimePasswordCachingPolicy = oneTimePasswordCachingPolicy;
        _smsDeliveryService = smsDeliveryService;
    }

    public async override Task<ApplicationResponse<HandleOneTimePasswordCommandResponse>> Handle(HandleOneTimePasswordCommandRequest request, CancellationToken cancellationToken)
    {
        OneTimePasswordHandlingOperationResult result = default;

        async Task GenerateAndPersistCode()
        {
            var passwordGenerationResult = await Mediator.Send(new GenerateOneTimePasswordCommandRequest { Resource = request.Resource });

            await _smsDeliveryService.SendAsync(passwordGenerationResult.Data.OneTimePassword);

            result = OneTimePasswordHandlingOperationResult.CodeGeneratedSuccessfully;
        }

        if (string.IsNullOrWhiteSpace(request.Code)) //It assumes that it needs to generate a code whenever the request doesn't receive one
            await GenerateAndPersistCode();
        else
        {
            var now = DateTime.UtcNow;
            var oneTimePasswordDto = await _oneTimePasswordRepository
                                    .GetAsync(request.UserId,
                                              request.Resource,
                                              cancellationToken: cancellationToken);
            if (oneTimePasswordDto is not null)
            {
                var expirationDateTime = oneTimePasswordDto.CreatedOn
                                                .AddSeconds(Math.Abs(_oneTimePasswordCachingPolicy.ExpirationTimeInSeconds))
                                                .AddSeconds(_oneTimePasswordCachingPolicy.ExpirationTimeOffsetInSeconds);

                if (now > expirationDateTime)
                    result = OneTimePasswordHandlingOperationResult.ExpiredCode;
                else if (string.CompareOrdinal(request.Code, oneTimePasswordDto.Code).Equals(0)) //Do the informed code and the generated one match?
                {
                    result = OneTimePasswordHandlingOperationResult.ValidCode;
                    await _oneTimePasswordRepository.RemoveAsync(request.UserId, request.Resource, cancellationToken);
                }
                else
                    result = OneTimePasswordHandlingOperationResult.InvalidCode;
            }
            else
                await GenerateAndPersistCode();
        }

        return ApplicationResponse<HandleOneTimePasswordCommandResponse>.Create(new() { Result = result });
    }
}
