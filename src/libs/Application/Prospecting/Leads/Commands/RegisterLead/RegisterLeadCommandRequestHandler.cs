using Application.Core.Contracts.Repository.Prospecting;
using Application.Prospecting.Leads.IntegrationEvents.LeadRegistered;
using Domain.Prospecting.Entities;
using MediatR;
using Shared.Events.EventDispatching;
using Shared.RequestHandling;
using Shared.Results;

namespace Application.Prospecting.Leads.Commands.RegisterLead;

internal sealed class RegisterLeadCommandRequestHandler : ApplicationRequestHandler<RegisterLeadCommandRequest, RegisterLeadCommandResponse>
{
    private readonly ILeadRepository _leadRepository;

    public RegisterLeadCommandRequestHandler(
        IMediator mediator,
        IEventDispatching eventDispatcher,
        ILeadRepository leadRepository) : base(mediator, eventDispatcher)
    {
        _leadRepository = leadRepository;
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

        AddEvent(new LeadRegisteredIntegrationEvent(lead.MapToDto()));

        return ApplicationResponse<RegisterLeadCommandResponse>.Create(new(lead.Id));
    }
}