using Application.Core.Contracts.Repository.Prospecting;
using Application.Core.Diagnostics;
using Application.Prospecting.Leads.Commands.RegisterLead;
using Application.Prospecting.Leads.IntegrationEvents.LeadBulkInserted;
using Application.Prospecting.Leads.Shared;
using CrossCutting.Csv;
using CrossCutting.FileStorage;
using CrossCutting.Security.IAM;
using Domain.Prospecting.Entities;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using OpenTelemetry;
using Shared.DataPagination;
using Shared.Diagnostics;
using Shared.Events.EventDispatching;
using Shared.RequestHandling;
using Shared.Results;
using System.Diagnostics;
using System.Globalization;
using System.Text;

namespace Application.Prospecting.Leads.Commands.BulkInsertLead;

internal sealed class BulkInsertLeadCommandRequestHandler : ApplicationRequestHandler<BulkInsertLeadCommandRequest, BulkInsertLeadCommandResponse>
{
    private readonly ICsvHelper _csvHelper;
    private readonly IValidator<RegisterLeadCommandRequest> _requestValidator;
    private readonly IFileStorageProvider _fileStorageProvider;
    private readonly IUserService _userService;
    private readonly ILeadRepository _leadRepository;

    public BulkInsertLeadCommandRequestHandler(
        IMediator mediator,
        IEventDispatching eventDispatcher,
        ICsvHelper csvHelper,
        IFileStorageProvider fileStorageProvider,
        ILeadRepository leadRepository,
        IUserService userService,
        IValidator<RegisterLeadCommandRequest> requestValidator) : base(mediator, eventDispatcher)
    {
        _csvHelper = csvHelper;
        _requestValidator = requestValidator;
        _fileStorageProvider = fileStorageProvider;
        _userService = userService;
        _leadRepository = leadRepository;
    }

    public async override Task<ApplicationResponse<BulkInsertLeadCommandResponse>> Handle(BulkInsertLeadCommandRequest request, CancellationToken cancellationToken)
    {
        await _fileStorageProvider.UploadAsync(
            request.InputStream,
            _userService.GetUserEmail() ?? "-",
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

        var existingLeads = await _leadRepository.GetAsync(
            default,
            PaginationOptions.SinglePage(),
            cancellationToken);
        if (existingLeads.Items.Any())
            upcomingLeads.ForEach(upcLead =>
            {
                if (existingLeads.Items.Any(exsLead =>  string.Compare(upcLead.Cnpj, exsLead.Cnpj, true, culture) == 0 ||
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
                        .Select(lead => Lead.Criar(lead.Cnpj!,
                                                 lead.RazaoSocial!,
                                                 lead.Cep!,
                                                 lead.Endereco!,
                                                 lead.Bairro!,
                                                 lead.Cidade!,
                                                 lead.Estado!,
                                                 lead.Numero,
                                                 lead.Complemento))
                        .ToList();

        await _leadRepository.AddRangeAsync(newLeads, cancellationToken);

        PushTelemetryData(newLeads);

		AddEvent(new LeadBulkInsertedIntegrationEvent(newLeads.MapToDtoList()));

        return ApplicationResponse<BulkInsertLeadCommandResponse>.Create(new());
    }

    private void PushTelemetryData(IEnumerable<Lead> newLeads)
    {
        var leadCount = newLeads.Count();

		//This counter is configured to be a metric and exported to Prometheus (see OpenTelemetryConfigurationExtensions.cs -> .WithMetrics -> mtr.AddPrometheusExporter())
		//This counter is also configured to be scraped by Prometheus via and endpoint that was set in Program.cs file (app.UseOpenTelemetryPrometheusScrapingEndpoint();)
		ApplicationDiagnostics.RegisteredLeadsCounter.Add(
		/*Add*/leadCount
		//,[
		//    new KeyValuePair<string, object?>("client.membership", client.Membership.ToString())
		//	  //add as many new tags as you see fit...
		//]
		//Adding kvps to the counter makes Grafana data grouping possible, for example
		);

		var handlerName = GetType().FullName!;
		var diagnosticsDataCollector = DiagnosticsDataCollector
										.WithActivity(Activity.Current)
										.WithTags(
											(ApplicationDiagnostics.Constants.LeadId, string.Join(',', newLeads.Select(l => l.Id))),
											(ApplicationDiagnostics.Constants.HandlerName, handlerName)
										)
										//Just experimenting C# 12 new sugar syntax in the next lines of code
										.WithTags([.. newLeads.Select(ld => ($"lead_{ld.Id}", ld.Id))])
										.WithTags([.. Enumerable.Range(1, leadCount).Select(c => ($"lead_{c}", newLeads.ElementAt(c - 1).RazaoSocial))])
										.WithBaggageData( //Useful for data Propagation
											Baggage.Current,
											(ApplicationDiagnostics.Constants.LeadId, string.Join(',', newLeads.Select(l => l.Id))),
											(ApplicationDiagnostics.Constants.HandlerName, handlerName)
										)
										.PushData();
	}
}