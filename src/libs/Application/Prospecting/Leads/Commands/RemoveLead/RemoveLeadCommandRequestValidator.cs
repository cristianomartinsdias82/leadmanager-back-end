using FluentValidation;

namespace Application.Prospecting.Leads.Commands.RemoveLead;

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