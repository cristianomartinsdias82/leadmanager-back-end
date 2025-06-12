using FluentValidation;

namespace Application.Reporting.Queries.DownloadGeneratedReport;

public sealed class DownloadGeneratedReportRequestValidator : AbstractValidator<DownloadGeneratedReportQueryRequest>
{
    public DownloadGeneratedReportRequestValidator()
    {
        RuleFor(request => request)
            .NotNull()
            .WithMessage("Requisição inválida.");

		RuleFor(request => request.Id)
			.GreaterThan(0)
			.WithMessage("Id do da requisição de geração do relatório é obrigatório.");
	}
}