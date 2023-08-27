using Application.Contracts.Caching;
using Application.Contracts.Persistence;
using Application.Features.Leads.IntegrationEvents.LeadUpdated;
using CrossCutting.MessageContracts;
using Core.DomainEvents.LeadUpdated;
using MediatR;
using Shared.Events.EventDispatching;
using Shared.RequestHandling;
using Shared.Results;

namespace Application.Features.Leads.Commands.UpdateLead;

internal sealed class UpdateLeadCommandHandler : ApplicationRequestHandler<UpdateLeadCommandRequest, UpdateLeadCommandResponse>
{
    private readonly ILeadManagerDbContext _dbContext;
    private readonly ICachingManagement _cachingManager;

    public UpdateLeadCommandHandler(
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
            return ApplicationResponse<UpdateLeadCommandResponse>.Create(null!, message: "Lead não encontrado.");

        lead.Atualizar(
            request.RazaoSocial!,
            request.Cep!,
            request.Endereco!,
            request.Cidade!,
            request.Estado!,
            request.Bairro!,
            request.Numero,
            request.Complemento);
        
        await _dbContext.SaveChangesAsync(cancellationToken);

        var leadDto = lead.AsMessageContract();
        await _cachingManager.UpdateLeadEntryAsync(leadDto, cancellationToken);

        AddEvent(new LeadUpdatedIntegrationEvent(leadDto));

        return ApplicationResponse<UpdateLeadCommandResponse>.Create(new UpdateLeadCommandResponse());
    }
}