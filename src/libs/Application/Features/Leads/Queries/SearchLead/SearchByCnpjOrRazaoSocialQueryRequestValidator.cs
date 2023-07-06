using FluentValidation;

namespace Application.Features.Leads.Queries.SearchByCnpjOrRazaoSocial;

public sealed class SearchByCnpjOrRazaoSocialRequestValidator : AbstractValidator<SearchByCnpjOrRazaoSocialQueryRequest>
{
    public SearchByCnpjOrRazaoSocialRequestValidator()
    {
        RuleFor(x => x.CnpjOrRazaoSocial)
            .NotEmpty()
            .WithMessage("Campo para pesquisa por Cnpj / Razão social é obrigatório.");
    }
}