namespace Domain.Prospecting.Entities;

public sealed record LeadDto
(
    Guid Id,
    string RazaoSocial,
    string Cnpj,
    string Endereco,
    string? Numero,
    string? Complemento,
    string Cep,
    string Cidade,
    string Bairro,
    string Estado,
    byte[] Revision
);