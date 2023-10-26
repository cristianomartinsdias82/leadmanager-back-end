using Application.Core.Contracts.Persistence;
using Application.Core.Contracts.Repository;
using Application.Prospecting.Leads.IntegrationEvents.LeadUpdated;
using Domain.Prospecting.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Events.EventDispatching;
using Shared.RequestHandling;
using Shared.Results;

namespace Application.Prospecting.Leads.Commands.UpdateLead;

internal sealed class UpdateLeadCommandRequestHandler : ApplicationRequestHandler<UpdateLeadCommandRequest, UpdateLeadCommandResponse>
{
    private const string Mensagem_FalhaAtualizacaoConcorrente = "Este registro foi atualizado por outro usuário antes desta operação.";
    private const string Mensagem_FalhaRemocaoConcorrente = "Este registro foi removido por outro usuário antes desta operação.";
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
            if (entry is not null)
            {
                var databaseValues = await entry.GetDatabaseValuesAsync(cancellationToken);
                if (databaseValues is not null)
                    return ApplicationResponse<UpdateLeadCommandResponse>.Create(
                        data: new UpdateLeadCommandResponse(
                                    RecordStates.Modified,
                                    new (databaseValues.GetValue<Guid>(nameof(Lead.Id)), databaseValues.GetValue<byte[]>(nameof(Lead.RowVersion))),
                                    ((Lead)databaseValues.ToObject()).MapToDto()),
                        message: Mensagem_FalhaAtualizacaoConcorrente,
                        operationCode: OperationCodes.ConcurrencyIssue);
            }

            return ApplicationResponse<UpdateLeadCommandResponse>.Create(
                data: new UpdateLeadCommandResponse(
                            RecordStates.Deleted,
                            default!,
                            default!),
                message: Mensagem_FalhaRemocaoConcorrente,
                operationCode: OperationCodes.ConcurrencyIssue);
        }

        AddEvent(new LeadUpdatedIntegrationEvent(lead.MapToDto()));

        return ApplicationResponse<UpdateLeadCommandResponse>.Create(
                new UpdateLeadCommandResponse(
                    default,
                    default,
                    default));
    }
}