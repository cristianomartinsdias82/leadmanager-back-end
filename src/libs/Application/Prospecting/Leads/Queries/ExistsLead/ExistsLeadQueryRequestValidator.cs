using FluentValidation;

namespace Application.Prospecting.Leads.Queries.ExistsLead;

public sealed class ExistsLeadQueryRequestValidator : AbstractValidator<ExistsLeadQueryRequest>
{
    public ExistsLeadQueryRequestValidator()
    {
        RuleFor(x => x.SearchTerm)
            .NotEmpty()
            .WithMessage("Termo para pesquisa é obrigatório.");
    }
}