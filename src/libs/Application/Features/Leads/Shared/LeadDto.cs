namespace Application.Features.Leads.Queries.Shared;

public record LeadDto(
    Guid Id,
    string Cnpj,
    string RazaoSocial,
    string Cep,
    string Endereco,
    string Bairro,
    string Cidade,
    string Estado,
    string? Numero,
    string? Complemento);
