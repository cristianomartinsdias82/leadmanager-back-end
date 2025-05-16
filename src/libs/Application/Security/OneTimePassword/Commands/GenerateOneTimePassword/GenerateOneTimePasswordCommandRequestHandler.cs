using Application.Core.Contracts.Repository.Security.OneTimePassword;
using Application.Security.OneTimePassword.IntegrationEvents.OneTimePasswordGenerated;
using CrossCutting.Security.IAM;
using CrossCutting.Security.Secrecy;
using MediatR;
using Shared.Events.EventDispatching;
using Shared.RequestHandling;
using Shared.Results;

namespace Application.Security.OneTimePassword.Commands.GenerateOneTimePassword;

internal sealed class GenerateOneTimePasswordCommandRequestHandler : ApplicationRequestHandler<GenerateOneTimePasswordCommandRequest, GenerateOneTimePasswordCommandResponse>
{
    private readonly IUserService _userService;
    private readonly ISecretGenerationService _secretGeneratorService;
    private readonly IOneTimePasswordRepository _oneTimePasswordRepository;
    private readonly TimeProvider _timeProvider;

    public GenerateOneTimePasswordCommandRequestHandler(
        IMediator mediator,
        IUserService userService,
        ISecretGenerationService secretGeneratorService,
        IOneTimePasswordRepository oneTimePasswordRepository,
        IEventDispatching eventDispatcher,
        TimeProvider timeProvider) : base(mediator, eventDispatcher)
    {
        _oneTimePasswordRepository = oneTimePasswordRepository;
        _userService = userService;
        _secretGeneratorService = secretGeneratorService;
        _timeProvider = timeProvider;
    }

    public async override Task<ApplicationResponse<GenerateOneTimePasswordCommandResponse>> Handle(GenerateOneTimePasswordCommandRequest request, CancellationToken cancellationToken)
    {
        var code = await _secretGeneratorService.GenerateAsync(
            minLength: 6,
            maxLength: 6,
            includeLowercaseLetters: false,
            includeUpperCaseLetters: false,
            includeNumbers: true,
            includeSpecialCharacters: false,
            cancellationToken);

        var userId = _userService.GetUserId()!.Value;

        await _oneTimePasswordRepository.SaveAsync(
            new(userId, request.Resource, _timeProvider.GetLocalNow(), code),
            cancellationToken);

        AddEvent(new OneTimePasswordGeneratedIntegrationEvent(userId, request.Resource, code));

        return ApplicationResponse<GenerateOneTimePasswordCommandResponse>.Create(new(code));
    }
}
