using FluentValidation;

namespace Application.Prospecting.Leads.Queries.GetLeadById;

public sealed class GetLeadByIdQueryRequestValidator : AbstractValidator<GetLeadByIdQueryRequest>
{
    public GetLeadByIdQueryRequestValidator()
    {
        RuleFor(request => request)
            .NotNull()
            .WithMessage("Requisição inválida.");

        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Campo Id é obrigatório.");
    }
}