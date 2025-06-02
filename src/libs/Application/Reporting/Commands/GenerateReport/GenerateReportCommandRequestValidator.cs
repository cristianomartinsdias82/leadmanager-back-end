using FluentValidation;

namespace Application.Reporting.Commands.GenerateReport;

public sealed class GenerateReportCommandRequestValidator : AbstractValidator<GenerateReportCommandRequest>
{
	public GenerateReportCommandRequestValidator()
	{
		RuleFor(request => request)
			.NotNull()
			.WithMessage("Requisição inválida.");

		RuleFor(request => request.ReportGenerationRequestArgs)
			.NotNull()
			.WithMessage("Requisição inválida.");

		RuleFor(request => request.ReceiverId)
			.NotEmpty()
			.WithMessage("Código do requisitante do relatório é obrigatório.");
    }
}