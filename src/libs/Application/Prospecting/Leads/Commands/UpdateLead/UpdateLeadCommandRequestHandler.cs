﻿using Application.Core.Contracts.Persistence;
using Application.Core.Contracts.Repository.Prospecting;
using Application.Core.Diagnostics;
using Application.Prospecting.Leads.IntegrationEvents.LeadUpdated;
using Domain.Prospecting.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry;
using Shared.Diagnostics;
using Shared.Events.EventDispatching;
using Shared.RequestHandling;
using Shared.Results;
using System.Diagnostics;

namespace Application.Prospecting.Leads.Commands.UpdateLead;

internal sealed class UpdateLeadCommandRequestHandler : ApplicationRequestHandler<UpdateLeadCommandRequest, UpdateLeadCommandResponse>
{
    private const string Mensagem_FalhaAtualizacaoConcorrente = "Este registro foi atualizado anteriormente por outro usuário.";
    private const string Mensagem_FalhaRemocaoConcorrente = "Este registro foi removido anteriormente por outro usuário.";
    private const string Mensagem_LeadNaoEncontrado = "Lead não encontrado.";
    private readonly ILeadRepository _leadRepository;

    public UpdateLeadCommandRequestHandler(
        IMediator mediator,
        IEventDispatching eventDispatcher,
        ILeadRepository leadRepository) : base(mediator, eventDispatcher)
    {
        _leadRepository = leadRepository;
    }

    public async override Task<ApplicationResponse<UpdateLeadCommandResponse>> Handle(UpdateLeadCommandRequest request, CancellationToken cancellationToken)
    {
        var lead = await _leadRepository.GetByIdAsync(request.Id!.Value, cancellationToken);
        if (lead is null)
            return ApplicationResponse<UpdateLeadCommandResponse>.Create(null!, message: Mensagem_LeadNaoEncontrado);

        lead.Atualizar(
            request.RazaoSocial!,
            request.Cep!,
            request.Endereco!,
            request.Bairro!,
            request.Cidade!,
            request.Estado!,
            request.Numero,
            request.Complemento);

        try
        {
            await _leadRepository.UpdateAsync(lead, request.Revision!, cancellationToken);
        }
        catch (DbUpdateConcurrencyException dbConcExc)
        {
            var entry = dbConcExc.Entries.SingleOrDefault();

			if (entry is not null) //If found, the record was previously updated
            {
                var databaseValues = await entry.GetDatabaseValuesAsync(cancellationToken);
                if (databaseValues is not null)
                    return ApplicationResponse<UpdateLeadCommandResponse>.Create(
                        data: new UpdateLeadCommandResponse(
                                    RecordStates.Modified,
                                    new (databaseValues.GetValue<Guid>(nameof(Lead.Id)), databaseValues.GetValue<byte[]>(nameof(Lead.RowVersion))),
                                    ((Lead)databaseValues.ToObject()).MapToDto()),
                        message: Mensagem_FalhaAtualizacaoConcorrente,
						inconsistencies: new Inconsistency("ConcurrencyIssue", string.Empty),
						operationCode: OperationCodes.ConcurrencyIssue);
            }

			//Otherwise, the record was previously deleted

			return ApplicationResponse<UpdateLeadCommandResponse>.Create(
                data: new UpdateLeadCommandResponse(
                            RecordStates.Deleted,
                            default!,
                            default!),
                exception: dbConcExc.AsExceptionData(),
                message: Mensagem_FalhaRemocaoConcorrente,
				inconsistencies: new Inconsistency("ConcurrencyIssue", string.Empty),
				operationCode: OperationCodes.ConcurrencyIssue);
        }

        PushTelemetryData(lead);

        AddEvent(new LeadUpdatedIntegrationEvent(lead.MapToDto()));

        return ApplicationResponse<UpdateLeadCommandResponse>.Create(
                new UpdateLeadCommandResponse(
                    default,
                    default,
                    default));
    }

	private void PushTelemetryData(Lead lead)
	{
		//This counter is configured to be a metric and exported to Prometheus (see OpenTelemetryConfigurationExtensions.cs -> .WithMetrics -> mtr.AddPrometheusExporter())
		//This counter is also configured to be scraped by Prometheus via and endpoint that was set in Program.cs file (app.UseOpenTelemetryPrometheusScrapingEndpoint();)
		ApplicationDiagnostics.UpdatedLeadsCounter.Add(
		/*Add*/1
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