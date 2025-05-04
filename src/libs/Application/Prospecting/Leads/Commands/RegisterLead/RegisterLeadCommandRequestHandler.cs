using Application.Core.Contracts.Repository.Prospecting;
using Application.Core.Contracts.Repository.UnitOfWork;
using Application.Core.Diagnostics;
using Application.Prospecting.Leads.IntegrationEvents.LeadRegistered;
using Domain.Prospecting.Entities;
using MediatR;
using OpenTelemetry;
using Shared.Diagnostics;
using Shared.Events.EventDispatching;
using Shared.RequestHandling;
using Shared.Results;
using System.Diagnostics;

namespace Application.Prospecting.Leads.Commands.RegisterLead;

internal sealed class RegisterLeadCommandRequestHandler : ApplicationRequestHandler<RegisterLeadCommandRequest, RegisterLeadCommandResponse>
{
    private readonly ILeadRepository _leadRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterLeadCommandRequestHandler(
        IMediator mediator,
        IEventDispatching eventDispatcher,
        ILeadRepository leadRepository,
		IUnitOfWork unitOfWork) : base(mediator, eventDispatcher)
    {
        _leadRepository = leadRepository;
		_unitOfWork = unitOfWork;
    }

    public async override Task<ApplicationResponse<RegisterLeadCommandResponse>> Handle(RegisterLeadCommandRequest request, CancellationToken cancellationToken)
    {
        var lead = Lead.Criar(
            request.Cnpj!,
            request.RazaoSocial!,
            request.Cep!,
            request.Endereco!,
            request.Bairro!,
            request.Cidade!,
            request.Estado!,
            request.Numero,
            request.Complemento
        );

        await _leadRepository.AddAsync(lead, cancellationToken);

		await _unitOfWork.CommitAsync(cancellationToken);

		PushTelemetryData(lead);

		AddEvent(new LeadRegisteredIntegrationEvent(lead.MapToDto()));

        return ApplicationResponse<RegisterLeadCommandResponse>.Create(new(lead.Id));
    }

    private void PushTelemetryData(Lead lead)
    {
		//This counter is configured to be a metric and exported to Prometheus (see OpenTelemetryConfigurationExtensions.cs -> .WithMetrics -> mtr.AddPrometheusExporter())
		//This counter is also configured to be scraped by Prometheus via an endpoint that was set in Program.cs file (app.UseOpenTelemetryPrometheusScrapingEndpoint();)
		ApplicationDiagnostics.RegisteredLeadsCounter.Add(
			/*Add*/1
		//,[
		//    new KeyValuePair<string, object?>("client.membership", client.Membership.ToString())
		//	//add as many new tags as you see fit...
		//]
		//Adding kvps to the counter makes Grafana data grouping possible, for example
		);

		var handlerName = GetType().FullName!;
		var diagnosticsDataCollector = DiagnosticsDataCollector
										.WithActivity(Activity.Current)
										.WithTags(
											(ApplicationDiagnostics.Constants.LeadId, lead.Id),
											(ApplicationDiagnostics.Constants.HandlerName, handlerName)
										)
										.WithBaggageData( //Useful for data Propagation
											(string name, string? value) => Baggage.SetBaggage(name, $"(Baggage item added from {GetType().Name}) -> {value}"),
											(ApplicationDiagnostics.Constants.LeadId, lead.Id.ToString()),
											(ApplicationDiagnostics.Constants.HandlerName, handlerName)
										)
										//.WithBaggageData( //Useful for data Propagation
										//	Baggage.Current,
										//	(ApplicationDiagnostics.Constants.LeadId, lead.Id.ToString()),
										//	(ApplicationDiagnostics.Constants.HandlerName, handlerName)
										//)
										.PushData();
	}
}