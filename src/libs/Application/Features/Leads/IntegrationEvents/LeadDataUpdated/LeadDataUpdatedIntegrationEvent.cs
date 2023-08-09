using Shared.IntegrationEvents;

namespace Application.Features.Leads.IntegrationEvents.LeadDataUpdated;

public sealed record LeadDataUpdatedIntegrationEvent(
    Guid LeadId,
    string Cnpj,
    string RazaoSocial,
    string Cep,
    string Endereco,
    string Cidade,
    string Estado,
    string Bairro,
    string? Numero,
    string? Complemento) : IIntegrationEvent;
