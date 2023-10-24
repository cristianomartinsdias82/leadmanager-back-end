using FluentValidation;

namespace Application.Prospecting.Addresses.Queries.SearchAddressByZipCode;

public sealed class SearchAddressByZipCodeQueryRequestValidator : AbstractValidator<SearchAddressByZipCodeQueryRequest>
{
    public SearchAddressByZipCodeQueryRequestValidator()
    {
        RuleFor(x => x.ZipCode)
            .NotEmpty()
            .WithMessage("Campo Cep é obrigatório.");

        //RuleFor(x => x.Cep)
        //    .Matches(@"/d{5}\-/d{3}")
        //    .WithMessage("Campo Cep inválido.");
    }
}