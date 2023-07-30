using Application.Contracts.Persistence;
using Application.Features.Leads.Commands.RegisterLead;
using Core.Entities;
using CrossCutting.Csv;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Shared.RequestHandling;
using Shared.Results;
using System.Globalization;
using System.Text;

namespace Application.Features.Leads.Commands.BulkInsertLead;

internal sealed class BulkInsertLeadCommandHandler : ApplicationRequestHandler<BulkInsertLeadCommandRequest, BulkInsertLeadCommandResponse>
{
    private readonly ILeadManagerDbContext _dbContext;
    private readonly ICsvHelper _csvHelper;
    private readonly IValidator<RegisterLeadCommandRequest> _requestValidator;

    public BulkInsertLeadCommandHandler(
        ILeadManagerDbContext dbContext,
        ICsvHelper csvHelper,
        IValidator<RegisterLeadCommandRequest> requestValidator)
    {
        _dbContext = dbContext;
        _csvHelper = csvHelper;
        _requestValidator = requestValidator;
    }

    public async override Task<ApplicationResponse<BulkInsertLeadCommandResponse>> Handle(BulkInsertLeadCommandRequest request, CancellationToken cancellationToken)
    {
        var upcomingLeads = new List<RegisterLeadCommandRequest>();
        var validationResult = default(ValidationResult);
        var index = 0;
        var inconsistencies = new List<Inconsistency>();
        var culture = CultureInfo.InvariantCulture;

        var items = _csvHelper.Fetch<RegisterLeadCommandRequest>(
            request.InputStream,
            encoding: Encoding.Latin1,
            cultureInfo: culture);
        foreach (var item in items)
        {
            ++index;
            validationResult = await _requestValidator.ValidateAsync(item);
            if (!validationResult.IsValid)
            {
                inconsistencies.Add(new($"Registro #{index}", string.Join(", ", validationResult.Errors.Select(err => err.ErrorMessage))));
                continue;
            }

            upcomingLeads.Add(item);
        }

        if (inconsistencies.Count > 0)
            return ApplicationResponse<BulkInsertLeadCommandResponse>.Create(
                new(),
                message: "Por favor, corrija o(s) erro(s) a seguir e tente novamente:",
                operationCode: OperationCodes.ValidationFailure,
                inconsistencies: inconsistencies.ToArray()
            );

        //TODO: Implement a Caching provider (cross-cutting) and retrieve the lead data from it in order to achieve better performance
        var existingLeads = await _dbContext.Leads.ToListAsync(cancellationToken);

        upcomingLeads.ForEach(upcLead =>
        {
            if (existingLeads.Any(exsLead => string.Compare(upcLead.Cnpj, exsLead.Cnpj, true, culture) == 0 ||
                                             string.Compare(upcLead.RazaoSocial, exsLead.RazaoSocial, true, culture) == 0))
                inconsistencies.Add(new(string.Empty, $"Lead {upcLead.Cnpj} - {upcLead.RazaoSocial} já existente."));
        });

        if (inconsistencies.Count > 0)
            return ApplicationResponse<BulkInsertLeadCommandResponse>.Create(
                new(),
                message: "Por favor, corrija o(s) erro(s) a seguir e tente novamente:",
                operationCode: OperationCodes.ValidationFailure,
                inconsistencies: inconsistencies.ToArray()
            );

        await _dbContext.Leads.AddRangeAsync(upcomingLeads
                                                .Select(lead => new Lead(lead.Cnpj!,
                                                                         lead.RazaoSocial!,
                                                                         lead.Cep!,
                                                                         lead.Endereco!,
                                                                         lead.Bairro!,
                                                                         lead.Cidade!,
                                                                         lead.Estado!,
                                                                         lead.Numero,
                                                                         lead.Complemento))
                                                .ToArray(),
                                            cancellationToken);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return ApplicationResponse<BulkInsertLeadCommandResponse>.Create(new());
    }
}