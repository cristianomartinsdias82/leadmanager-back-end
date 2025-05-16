using FluentValidation;

namespace Application.Prospecting.Leads.Queries.DownloadLeadsFile;

public sealed class DownloadLeadsFileQueryRequestValidator : AbstractValidator<DownloadLeadsFileQueryRequest>
{
    public DownloadLeadsFileQueryRequestValidator()
    {
        RuleFor(request => request)
            .NotNull()
            .WithMessage("Requisição inválida.");

        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Campo Id é obrigatório.");
    }
}