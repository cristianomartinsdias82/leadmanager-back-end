using FluentValidation;

namespace Application.Reporting.Commands.RemoveRequestedReport;

public sealed class RemoveRequestedReportCommandRequestValidator : AbstractValidator<RemoveRequestedReportCommandRequest>
{
    public RemoveRequestedReportCommandRequestValidator()
    {
        RuleFor(request => request)
            .NotNull()
            .WithMessage("Requisição inválida.");

        RuleFor(request => request.Id)
            .GreaterThan(0)
			.WithMessage("Id do da requisição de geração do relatório é obrigatório.");
	}
}