using Application.Contracts.Persistence;
using Application.Features.Leads.IntegrationEvents.LeadDataUpdated;
using Core.DomainEvents.LeadDataUpdated;
using MediatR;
using Shared.Events.EventDispatching;
using Shared.RequestHandling;
using Shared.Results;

namespace Application.Features.Leads.Commands.UpdateLead;

internal sealed class UpdateLeadCommandHandler : ApplicationRequestHandler<UpdateLeadCommandRequest, UpdateLeadCommandResponse>
{
    private readonly ILeadManagerDbContext _dbContext;

    public UpdateLeadCommandHandler(
        IMediator mediator,
        IEventDispatching eventDispatcher,
        ILeadManagerDbContext dbContext) : base(mediator, eventDispatcher)
    {
        _dbContext = dbContext;
    }

    public async override Task<ApplicationResponse<UpdateLeadCommandResponse>> Handle(UpdateLeadCommandRequest request, CancellationToken cancellationToken)
    {
        var lead = await _dbContext.Leads.FindAsync(request.Id);
        if (lead is null)
            return ApplicationResponse<UpdateLeadCommandResponse>.Create(null!, message: "Lead não encontrado.");

        lead.Atualizar(
            request.RazaoSocial!,
            request.Cnpj!,
            request.Cep!,
            request.Endereco!,
            request.Cidade!,
            request.Estado!,
            request.Bairro!,
            request.Numero,
            request.Complemento);
        
        await _dbContext.SaveChangesAsync(cancellationToken);

        AddEvent(new LeadDataUpdatedDomainEvent(
                        lead.Id,
                        lead.Cnpj,
                        lead.RazaoSocial,
                        lead.Cep,
                        lead.Logradouro,
                        lead.Cidade,
                        lead.Estado,
                        lead.Bairro,
                        lead.Numero,
                        lead.Complemento));

        AddEvent(new LeadDataUpdatedIntegrationEvent(
                        lead.Id,
                        lead.Cnpj,
                        lead.RazaoSocial,
                        lead.Cep,
                        lead.Logradouro,
                        lead.Cidade,
                        lead.Estado,
                        lead.Bairro,
                        lead.Numero,
                        lead.Complemento));

        return ApplicationResponse<UpdateLeadCommandResponse>.Create(new UpdateLeadCommandResponse());
    }
}