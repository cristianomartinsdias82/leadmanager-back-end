using FluentValidation;
using static Application.Security.LeadManagerSecurityConfiguration;

namespace Application.Security.OneTimePassword.Commands.HandleOneTimePassword;

public sealed class HandleOneTimePasswordCommandRequestValidator : AbstractValidator<HandleOneTimePasswordCommandRequest>
{
    private static readonly string[] Permissions = new string[]
    {
        Claims.Read,
        Claims.Insert,
        Claims.BulkInsert,
        Claims.Update,
        Claims.Delete
    };

    public HandleOneTimePasswordCommandRequestValidator()
    {
        RuleFor(request => request)
            .NotNull()
            .WithMessage("Requisição inválida.");

        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("Id do usuário é inválido.");

        RuleFor(x => x.Resource)
            .NotEmpty()
            .WithMessage("Campo Resource é obrigatório.");

        When(x => !string.IsNullOrWhiteSpace(x.Resource), () =>
        {
            RuleFor(x => x.Resource)
                .Must(res => Permissions.Contains(res))
                .WithMessage("Campo Resource é inválido.");
        });
    }
}