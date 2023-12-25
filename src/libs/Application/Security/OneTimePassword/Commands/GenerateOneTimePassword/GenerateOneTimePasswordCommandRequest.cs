using MediatR;
using Shared.Results;

namespace Application.Security.OneTimePassword.Commands.GenerateOneTimePassword;

public sealed record GenerateOneTimePasswordCommandRequest : IRequest<ApplicationResponse<GenerateOneTimePasswordCommandResponse>>
{
    public required string Resource { get; init; }
}