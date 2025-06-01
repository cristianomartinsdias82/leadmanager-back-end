using FluentValidation;
using Shared.Reporting;

namespace Application.Reporting.Commands.RequestReportGeneration;

public sealed class RequestReportGenerationCommandRequestValidator : AbstractValidator<RequestReportGenerationCommandRequest>
{
    public RequestReportGenerationCommandRequestValidator()
    {
        RuleFor(request => request)
            .NotNull()
            .WithMessage("Requisição inválida.");

        RuleFor(request => request.Format)
            .NotEmpty()
            .WithMessage("Formato relatório é obrigatório.");
                    
        When(request => !string.IsNullOrWhiteSpace(request.Format), () =>
        {
            RuleFor(request => request.Format)
                .Must(format => Enum.TryParse<ReportGenerationFormats>(format, true, out var _))
                .WithMessage("Arquivo inválido.");
        });
    }
}