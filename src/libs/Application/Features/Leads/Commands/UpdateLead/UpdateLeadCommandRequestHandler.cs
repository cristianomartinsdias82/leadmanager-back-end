using Application.Contracts.Caching;
using Application.Contracts.Persistence;
using Application.Features.Leads.IntegrationEvents.LeadUpdated;
using Application.Features.Leads.Shared;
using Core.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Events.EventDispatching;
using Shared.RequestHandling;
using Shared.Results;

namespace Application.Features.Leads.Commands.UpdateLead;

internal sealed class UpdateLeadCommandRequestHandler : ApplicationRequestHandler<UpdateLeadCommandRequest, UpdateLeadCommandResponse>
{
    private const string Mensagem_FalhaAtualizacaoConcorrente = "Este registro foi atualizado por outro usuário antes desta operação.";
    private const string Mensagem_FalhaRemocaoConcorrente = "Este registro foi removido por outro usuário antes desta operação.";
    private const string Mensagem_LeadNaoEncontrado = "Lead não encontrado.";
    private readonly ILeadManagerDbContext _dbContext;
    private readonly ICachingManagement _cachingManager;

    public UpdateLeadCommandRequestHandler(
        IMediator mediator,
        IEventDispatching eventDispatcher,
        ILeadManagerDbContext dbContext,
        ICachingManagement cachingManager) : base(mediator, eventDispatcher)
    {
        _cachingManager = cachingManager;
        _dbContext = dbContext;
    }

    public async override Task<ApplicationResponse<UpdateLeadCommandResponse>> Handle(UpdateLeadCommandRequest request, CancellationToken cancellationToken)
    {
        var lead = await _dbContext.Leads.FindAsync(request.Id);
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
            _dbContext.SetConcurrencyToken(lead, nameof(Lead.RowVersion), request.Revision!);
            await _dbContext.SaveChangesAsync(cancellationToken);
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

        var leadDto = lead.MapToDto();

        await _cachingManager.UpdateLeadEntryAsync(leadDto, cancellationToken);

        AddEvent(new LeadUpdatedIntegrationEvent(leadDto));

        return ApplicationResponse<UpdateLeadCommandResponse>.Create(
                new UpdateLeadCommandResponse(
                    default,
                    default,
                    default));
    }
}