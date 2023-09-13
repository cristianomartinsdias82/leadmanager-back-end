using Application.Contracts.Caching;
using Application.Contracts.Persistence;
using Application.Features.Leads.IntegrationEvents.LeadRemoved;
using Application.Features.Leads.Shared;
using Core.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Events.EventDispatching;
using Shared.RequestHandling;
using Shared.Results;

namespace Application.Features.Leads.Commands.RemoveLead;

internal sealed class RemoveLeadCommandRequestHandler : ApplicationRequestHandler<RemoveLeadCommandRequest, RemoveLeadCommandResponse>
{
    private const string Mensagem_FalhaAtualizacaoConcorrente = "Este registro foi atualizado por outro usuário antes desta operação.";
    private const string Mensagem_FalhaRemocaoConcorrente = "Este registro foi removido por outro usuário antes desta operação.";
    private const string Mensagem_LeadNaoEncontrado = "Lead não encontrado.";
    private readonly ILeadManagerDbContext _dbContext;
    private readonly ICachingManagement _cachingManager;

    public RemoveLeadCommandRequestHandler(
        IMediator mediator,
        IEventDispatching eventDispatcher,
        ILeadManagerDbContext dbContext,
        ICachingManagement cachingManager) : base(mediator, eventDispatcher)
    {
        _dbContext = dbContext;
        _cachingManager = cachingManager;
    }

    public async override Task<ApplicationResponse<RemoveLeadCommandResponse>> Handle(RemoveLeadCommandRequest request, CancellationToken cancellationToken)
    {
        var lead = await _dbContext.Leads.FindAsync(request.Id);
        if (lead is null)
            return ApplicationResponse<RemoveLeadCommandResponse>.Create(default!, message: Mensagem_LeadNaoEncontrado);

        _dbContext.Leads.Remove(lead);

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
                    return ApplicationResponse<RemoveLeadCommandResponse>.Create(
                        data: new RemoveLeadCommandResponse(
                                    RecordStates.Modified,
                                    new(databaseValues.GetValue<Guid>(nameof(Lead.Id)), databaseValues.GetValue<byte[]>(nameof(Lead.RowVersion))),
                                    ((Lead)databaseValues.ToObject()).MapToDto()),
                        message: Mensagem_FalhaAtualizacaoConcorrente,
                        operationCode: OperationCodes.ConcurrencyIssue);
            }

            return ApplicationResponse<RemoveLeadCommandResponse>.Create(
                data: new RemoveLeadCommandResponse(
                            RecordStates.Deleted,
                            default!,
                            default!),
                message: Mensagem_FalhaRemocaoConcorrente,
                operationCode: OperationCodes.ConcurrencyIssue);
        }

        var leadDto = lead.MapToDto();

        await _cachingManager.RemoveLeadEntryAsync(leadDto, cancellationToken);

        AddEvent(new LeadRemovedIntegrationEvent(leadDto));

        return ApplicationResponse<RemoveLeadCommandResponse>.Create(
            new RemoveLeadCommandResponse(
                default,
                default,
                default));
    }
}