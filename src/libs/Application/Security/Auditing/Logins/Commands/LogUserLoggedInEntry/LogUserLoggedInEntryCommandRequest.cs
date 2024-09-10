using MediatR;
using Shared.Results;

namespace Application.Security.Auditing.Logins.Commands.LogUserLoggedInEntry;

public sealed record LogUserLoggedInEntryCommandRequest : IRequest<ApplicationResponse<LogUserLoggedInEntryCommandResponse>>
{
}
