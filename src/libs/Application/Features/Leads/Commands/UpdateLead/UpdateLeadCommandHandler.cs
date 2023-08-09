using Application.Contracts.Persistence;
using Application.Features.Leads.IntegrationEvents.LeadDataUpdated;
using Core.Entities.DomainEvents;
using MediatR;
using Shared.Helpers;
using Shared.RequestHandling;
using Shared.Results;

namespace Application.Features.Leads.Commands.UpdateLead;

internal sealed class UpdateLeadCommandHandler : ApplicationRequestHandler<UpdateLeadCommandRequest, UpdateLeadCommandResponse>
{
    private readonly ILeadManagerDbContext _dbContext;
    private readonly IPublisher _publisher;

    public UpdateLeadCommandHandler(
        ILeadManagerDbContext dbContext,
        IPublisher publisher)
    {
        _dbContext = dbContext;
        _publisher = publisher;
    }

    public async override Task<ApplicationResponse<UpdateLeadCommandResponse>> Handle(UpdateLeadCommandRequest request, CancellationToken cancellationToken)
    {
        var outdatedLead = await _dbContext.Leads.FindAsync(request.Id);
        if (outdatedLead is null)
            return ApplicationResponse<UpdateLeadCommandResponse>.Create(null!, message: "Lead não encontrado.");

        outdatedLead.Atualizar(
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

        await TaskHelper.WhenAll(
            Task.Run(() =>
            {
                _publisher.Publish(
                    new LeadDataUpdatedDomainEvent(
                        outdatedLead.Id,
                        outdatedLead.Cnpj,
                        outdatedLead.RazaoSocial,
                        outdatedLead.Cep,
                        outdatedLead.Logradouro,
                        outdatedLead.Cidade,
                        outdatedLead.Estado,
                        outdatedLead.Bairro,
                        outdatedLead.Numero,
                        outdatedLead.Complemento),
                    cancellationToken);
            }),
            Task.Run(() =>
            {
                _publisher.Publish(
                    new LeadDataUpdatedIntegrationEvent(
                        outdatedLead.Id,
                        outdatedLead.Cnpj,
                        outdatedLead.RazaoSocial,
                        outdatedLead.Cep,
                        outdatedLead.Logradouro,
                        outdatedLead.Cidade,
                        outdatedLead.Estado,
                        outdatedLead.Bairro,
                        outdatedLead.Numero,
                        outdatedLead.Complemento),
                    cancellationToken);
            })
        );

        return ApplicationResponse<UpdateLeadCommandResponse>.Create(new UpdateLeadCommandResponse());
    }
}