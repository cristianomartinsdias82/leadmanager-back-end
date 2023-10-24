using FluentValidation;

namespace Application.Prospecting.Leads.Queries.SearchLead;

public sealed class SearchLeadQueryRequestValidator : AbstractValidator<SearchLeadQueryRequest>
{
    public SearchLeadQueryRequestValidator()
    {
        RuleFor(x => x.SearchTerm)
            .NotEmpty()
            .WithMessage("Termo para pesquisa é obrigatório.");
    }
}