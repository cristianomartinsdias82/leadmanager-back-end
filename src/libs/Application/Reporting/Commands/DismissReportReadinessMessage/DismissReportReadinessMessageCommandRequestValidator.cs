using FluentValidation;
using Shared.Exportation;

namespace Application.Reporting.Commands.DismissReportReadinessMessage;

public sealed class DismissReportReadinessMessageCommandRequestValidator : AbstractValidator<DismissReportReadinessMessageCommandRequest>
{
    public DismissReportReadinessMessageCommandRequestValidator()
    {
        RuleFor(request => request)
            .NotNull()
            .WithMessage("Requisição inválida.");

        RuleFor(request => request.Id)
            .GreaterThan(0)
            .WithMessage("Id do da requisição de geração do relatório é obrigatório.");
    }
}