using FluentValidation;
using static Application.Security.LeadManagerSecurityConfiguration;

namespace Application.Security.OneTimePassword.Commands.GenerateOneTimePassword;

public sealed class GenerateOneTimePasswordCommandRequestValidator : AbstractValidator<GenerateOneTimePasswordCommandRequest>
{
    private static readonly string[] Permissions = new string[]
    {
        Claims.Read,
        Claims.Insert,
        Claims.BulkInsert,
        Claims.Update,
        Claims.Delete
    };

    public GenerateOneTimePasswordCommandRequestValidator()
    {
        RuleFor(request => request)
            .NotNull()
            .WithMessage("Requisição inválida.");

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