using FluentValidation;

namespace Application.Features.Leads.Commands.RemoveLead;

public sealed class RemoveLeadCommandRequestValidator : AbstractValidator<RemoveLeadCommandRequest>
{
    public RemoveLeadCommandRequestValidator()
    {
        RuleFor(lead => lead)
            .NotNull()
            .WithMessage("Requisição inválida.");

        RuleFor(lead => lead.Id)
            .NotEmpty()
            .WithMessage("Campo Id é obrigatório.");
    }
}