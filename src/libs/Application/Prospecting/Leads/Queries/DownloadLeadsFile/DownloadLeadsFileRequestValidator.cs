using FluentValidation;

namespace Application.Prospecting.Leads.Queries.DownloadLeadsFile;

public sealed class DownloadLeadsFileRequestValidator : AbstractValidator<DownloadLeadsFileQueryRequest>
{
    public DownloadLeadsFileRequestValidator()
    {
        RuleFor(request => request)
            .NotNull()
            .WithMessage("Requisição inválida.");

        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Campo Id é obrigatório.");
    }
}