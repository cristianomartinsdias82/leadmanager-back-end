using Application.Core.Contracts.Persistence;
using Application.Core.Contracts.Repository.Prospecting;
using Application.Core.Diagnostics;
using Application.Prospecting.Leads.IntegrationEvents.LeadRemoved;
using Domain.Prospecting.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry;
using Shared.Diagnostics;
using Shared.Events.EventDispatching;
using Shared.RequestHandling;
using Shared.Results;
using System.Diagnostics;

namespace Application.Prospecting.Leads.Commands.RemoveLead;

internal sealed class RemoveLeadCommandRequestHandler : ApplicationRequestHandler<RemoveLeadCommandRequest, RemoveLeadCommandResponse>
{
    private const string Mensagem_FalhaAtualizacaoConcorrente = "Este registro foi atualizado anteriormente por outro usuário.";
    private const string Mensagem_LeadRemovido = "Este registro foi removido por outro usuário.";
    private const string Mensagem_LeadNaoEncontrado = "Lead não encontrado.";
    private readonly ILeadRepository _leadRepository;

    public RemoveLeadCommandRequestHandler(
        IMediator mediator,
        ILeadRepository leadRepository,
        IEventDispatching eventDispatcher) : base(mediator, eventDispatcher)
    {
        _leadRepository = leadRepository;
    }

    public async override Task<ApplicationResponse<RemoveLeadCommandResponse>> Handle(RemoveLeadCommandRequest request, CancellationToken cancellationToken)
    {
        var lead = await _leadRepository.GetByIdAsync(
                                                    request.Id,
                                                    bypassCacheLayer: true,
                                                    cancellationToken);
        if (lead is null)
            return ApplicationResponse<RemoveLeadCommandResponse>
                    .Create(default!, message: Mensagem_LeadNaoEncontrado);

        try
        {
            await _leadRepository.RemoveAsync(lead, cancellationToken);
        }
        catch (DbUpdateConcurrencyException dbConcExc)
        {
            var entry = dbConcExc.Entries.SingleOrDefault();
            if (entry is not null) //If found, the record was previously updated
			{
                var databaseValues = await entry.GetDatabaseValuesAsync(cancellationToken);
                if (databaseValues is not null)
                    return ApplicationResponse<RemoveLeadCommandResponse>.Create(
                        data: new RemoveLeadCommandResponse(
                                    RecordStates.Modified,
                                    new(databaseValues.GetValue<Guid>(nameof(Lead.Id)), databaseValues.GetValue<byte[]>(nameof(Lead.RowVersion))),
                                    ((Lead)databaseValues.ToObject()).MapToDto()),
                        message: Mensagem_FalhaAtualizacaoConcorrente,
						inconsistencies: new Inconsistency("ConcurrencyIssue", string.Empty),
						operationCode: OperationCodes.ConcurrencyIssue);
            }
            else //The record was previously deleted - TODO: Ver como o front se comporta com este bloco else. Fiz ele para passar nos testes unitários existentes na base de código
				return ApplicationResponse<RemoveLeadCommandResponse>.Create(
						data: new RemoveLeadCommandResponse(
									RecordStates.Deleted,
									default,
									lead.MapToDto()),
						message: Mensagem_LeadRemovido,
						inconsistencies: new Inconsistency("ConcurrencyIssue", string.Empty),
                        exception: dbConcExc.AsExceptionData(),
						operationCode: OperationCodes.ConcurrencyIssue);
		}

        await PushTelemetryData(lead);

        AddEvent(new LeadRemovedIntegrationEvent(lead.MapToDto()));

        return ApplicationResponse<RemoveLeadCommandResponse>.Create(
            new RemoveLeadCommandResponse(
                default,
                default,
                default));
    }

	private async ValueTask PushTelemetryData(Lead lead)
	{
		//This counter is configured to be a metric and exported to Prometheus (see OpenTelemetryConfigurationExtensions.cs -> .WithMetrics -> mtr.AddPrometheusExporter())
		//This counter is also configured to be scraped by Prometheus via and endpoint that was set in Program.cs file (app.UseOpenTelemetryPrometheusScrapingEndpoint();)
		ApplicationDiagnostics.RemovedLeadsCounter.Add(
		/*Add*/1
		//,[
		//    new KeyValuePair<string, object?>("client.membership", client.Membership.ToString())
		//	  //add as many new tags as you see fit...
		//]
		//Adding kvps to the counter makes Grafana data grouping possible, for example
		);

		var handlerName = GetType().FullName!;
		await DiagnosticsDataCollector
			.WithActivity(Activity.Current)
			.WithTags(
				(ApplicationDiagnostics.Constants.LeadId, lead.Id),
				(ApplicationDiagnostics.Constants.HandlerName, handlerName)
			)
			.WithBaggageData( //Useful for data Propagation
				Baggage.Current,
				(ApplicationDiagnostics.Constants.LeadId, lead.Id.ToString()),
				(ApplicationDiagnostics.Constants.HandlerName, handlerName)
			)
			.PushData();
	}
}