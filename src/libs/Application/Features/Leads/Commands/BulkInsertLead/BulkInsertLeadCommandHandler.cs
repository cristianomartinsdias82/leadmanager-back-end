using Application.Contracts.Caching;
using Application.Contracts.Persistence;
using Application.Features.Leads.Commands.RegisterLead;
using Application.Features.Leads.IntegrationEvents.LeadBulkInserted;
using Application.Features.Leads.Shared;
using Core.Entities;
using CrossCutting.Csv;
using CrossCutting.FileStorage;
using CrossCutting.MessageContracts;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Events.EventDispatching;
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
    private readonly IFileStorageProvider _fileStorageProvider;
    private readonly ICachingManagement _cachingManager;

    public BulkInsertLeadCommandHandler(
        IMediator mediator,
        IEventDispatching eventDispatcher,
        ILeadManagerDbContext dbContext,
        ICachingManagement cachingManager,
        ICsvHelper csvHelper,
        IFileStorageProvider fileStorageProvider,
        IValidator<RegisterLeadCommandRequest> requestValidator) : base(mediator, eventDispatcher)
    {
        _dbContext = dbContext;
        _cachingManager = cachingManager;
        _csvHelper = csvHelper;
        _requestValidator = requestValidator;
        _fileStorageProvider = fileStorageProvider;
    }

    public async override Task<ApplicationResponse<BulkInsertLeadCommandResponse>> Handle(BulkInsertLeadCommandRequest request, CancellationToken cancellationToken)
    {
        await _fileStorageProvider.UploadAsync(
            request.InputStream,
            Guid.NewGuid().ToString(), //TODO: Replace this code with UserId from UserService dependency when it's available
            cancellationToken: cancellationToken);

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

        var existingLeads = await _dbContext.Leads.ToListAsync(cancellationToken);
        if (existingLeads.Count > 0)
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

        var newLeads = upcomingLeads
                        .Select(lead => new Lead(lead.Cnpj!,
                                                 lead.RazaoSocial!,
                                                 lead.Cep!,
                                                 lead.Endereco!,
                                                 lead.Bairro!,
                                                 lead.Cidade!,
                                                 lead.Estado!,
                                                 lead.Numero,
                                                 lead.Complemento))
                        .ToList();

        await _dbContext.Leads.AddRangeAsync(newLeads, cancellationToken);

        await _dbContext.SaveChangesAsync(cancellationToken);

        var leads = newLeads.AsDtoList();
        await _cachingManager.AddLeadEntriesAsync(leads, cancellationToken);

        var cachedLeads = await _cachingManager.GetLeadsAsync(new(), cancellationToken);

        AddEvent(new LeadBulkInsertedIntegrationEvent(leads));

        return ApplicationResponse<BulkInsertLeadCommandResponse>.Create(new());
    }
}