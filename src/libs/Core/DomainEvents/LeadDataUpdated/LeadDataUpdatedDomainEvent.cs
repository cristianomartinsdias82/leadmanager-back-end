using Shared.Events.DomainEvents;

namespace Core.DomainEvents.LeadDataUpdated;

public sealed record LeadDataUpdatedDomainEvent(
    Guid LeadId,
    string Cnpj,
    string RazaoSocial,
    string Cep,
    string Endereco,
    string Cidade,
    string Estado,
    string Bairro,
    string? Numero,
    string? Complemento) : IDomainEvent;