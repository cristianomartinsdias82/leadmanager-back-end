﻿using Application.Contracts.Caching;
using Application.Contracts.Persistence;
using Application.Features.Leads.IntegrationEvents.LeadRegistered;
using Core.Entities;
using CrossCutting.MessageContracts;
using MediatR;
using Shared.Events.EventDispatching;
using Shared.RequestHandling;
using Shared.Results;

namespace Application.Features.Leads.Commands.RegisterLead;

internal sealed class RegisterLeadCommandHandler : ApplicationRequestHandler<RegisterLeadCommandRequest, RegisterLeadCommandResponse>
{
    private readonly ILeadManagerDbContext _dbContext;
    private readonly ICachingManagement _cachingManager;

    public RegisterLeadCommandHandler(
        IMediator mediator,
        IEventDispatching eventDispatcher,
        ILeadManagerDbContext dbContext,
        ICachingManagement cachingManager) : base(mediator, eventDispatcher)
    {
        _dbContext = dbContext;
        _cachingManager = cachingManager;
    }

    public async override Task<ApplicationResponse<RegisterLeadCommandResponse>> Handle(RegisterLeadCommandRequest request, CancellationToken cancellationToken)
    {
        var lead = new Lead(
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

        await _dbContext.Leads.AddAsync(lead);
        await _dbContext.SaveChangesAsync(cancellationToken);

        var leadDto = lead.AsMessageContract();
        await _cachingManager.AddLeadEntryAsync(leadDto, cancellationToken);

        AddEvent(new LeadRegisteredIntegrationEvent(leadDto));

        return ApplicationResponse<RegisterLeadCommandResponse>.Create(new(lead.Id));
    }
}