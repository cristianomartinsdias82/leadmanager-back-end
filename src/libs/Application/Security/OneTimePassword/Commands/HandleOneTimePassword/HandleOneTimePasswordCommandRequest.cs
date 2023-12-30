using MediatR;
using Shared.Results;

namespace Application.Security.OneTimePassword.Commands.HandleOneTimePassword;

public sealed record HandleOneTimePasswordCommandRequest : IRequest<ApplicationResponse<HandleOneTimePasswordCommandResponse>>
{
    public required Guid UserId { get; init; }
    public required string Resource { get; init; }
    public string? Code { get; set; }
}