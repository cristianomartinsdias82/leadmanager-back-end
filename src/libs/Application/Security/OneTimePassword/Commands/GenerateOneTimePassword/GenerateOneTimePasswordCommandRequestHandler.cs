using Application.Core.Contracts.Repository.Security;
using CrossCutting.Security.IAM;
using CrossCutting.Security.Secrecy;
using MediatR;
using Shared.RequestHandling;
using Shared.Results;

namespace Application.Security.OneTimePassword.Commands.GenerateOneTimePassword;

public sealed class GenerateOneTimePasswordCommandRequestHandler : ApplicationRequestHandler<GenerateOneTimePasswordCommandRequest, GenerateOneTimePasswordCommandResponse>
{
    private readonly IUserService _userService;
    private readonly ISecretGenerationService _secretGeneratorService;
    private readonly IOneTimePasswordRepository _oneTimePasswordRepository;

    public GenerateOneTimePasswordCommandRequestHandler(
        IMediator mediator,
        IUserService userService,
        ISecretGenerationService secretGeneratorService,
        IOneTimePasswordRepository oneTimePasswordRepository) : base(mediator, default!)
    {
        _oneTimePasswordRepository = oneTimePasswordRepository;
        _userService = userService;
        _secretGeneratorService = secretGeneratorService;
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

        await _oneTimePasswordRepository.SaveAsync(
            new(_userService.GetUserId()!.Value, request.Resource, DateTime.UtcNow, code),
            cancellationToken);

        return ApplicationResponse<GenerateOneTimePasswordCommandResponse>.Create(new(code));
    }
}
