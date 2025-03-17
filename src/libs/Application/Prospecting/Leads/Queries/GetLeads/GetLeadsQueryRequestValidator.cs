using FluentValidation;

namespace Application.Prospecting.Leads.Queries.GetLeads;

public sealed class GetLeadsQueryRequestValidator : AbstractValidator<GetLeadsQueryRequest>
{
    public GetLeadsQueryRequestValidator()
    {
        When(x => !string.IsNullOrWhiteSpace(x.Search), () =>
        {
            RuleFor(x => x.Search!.Length)
                .LessThanOrEqualTo(100)
                .NotEmpty()
                .WithMessage("Termo para pesquisa deve conter até 100 caracteres.");
        });
    }
}