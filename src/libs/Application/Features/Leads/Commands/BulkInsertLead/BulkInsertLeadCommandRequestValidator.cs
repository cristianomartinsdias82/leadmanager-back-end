using FluentValidation;
using Microsoft.Extensions.Configuration;

namespace Application.Features.Leads.Commands.BulkInsertLead;

public sealed class BulkInsertLeadCommandRequestValidator : AbstractValidator<BulkInsertLeadCommandRequest>
{
    private static readonly string[] AcceptedContentTypes = new string[]
    {
        "text/csv",
        "application/vnd.ms-excel"
    };
    private readonly IConfiguration _configuration = default!;

    public BulkInsertLeadCommandRequestValidator(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public BulkInsertLeadCommandRequestValidator()
    {
        RuleFor(lead => lead)
            .NotNull()
            .WithMessage("Requisição inválida.");

        RuleFor(lead => lead.FileUpload_MaxSizeInBytesConfigurationKeyName)
            .NotEmpty()
            .WithMessage("Requisição inválida.");
                    
        RuleFor(lead => lead.InputStream)
            .NotNull()
            .WithMessage("Arquivo é obrigatório.");

        RuleFor(lead => lead.ContentType)
            .NotNull()
            .WithMessage("Requisição inválida.");

        RuleFor(lead => lead.FileName)
            .NotNull()
            .WithMessage("Requisição inválida.");

        When(lead => !string.IsNullOrWhiteSpace(lead.ContentType), () =>
        {
            RuleFor(lead => lead.ContentType)
                .Must((contentType) => Array.IndexOf(AcceptedContentTypes, contentType) > -1)
                .WithMessage("Arquivo inválido.");
        });

        When(lead => !string.IsNullOrWhiteSpace(lead.FileUpload_MaxSizeInBytesConfigurationKeyName), () => {

            RuleFor(lead => lead.ContentSizeInBytes)
                .Must((request, contentSizeInBytes) => {
                    var maxAllowedSize = int.Parse(_configuration[request.FileUpload_MaxSizeInBytesConfigurationKeyName]!);

                    return contentSizeInBytes <= maxAllowedSize;
                })
                .WithMessage("O tamanho do arquivo ultrapassa o tamanho máximo permitido.")
                .Must(size => size > 0)
                .WithMessage("Arquivo inválido.");
        });
    }
}