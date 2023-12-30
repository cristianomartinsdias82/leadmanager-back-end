using Domain.Prospecting.Entities;

namespace CrossCutting.MessageContracts;

public static class LeadExtensions
{
    public static List<LeadData> MapToMessageContractList(this IEnumerable<Lead> leads)
            => leads?
            .Select(MapToMessageContract)
            .ToList() ?? new List<LeadData>();

    public static LeadData MapToMessageContract(this Lead lead)
        => new()
        {
            Id = lead.Id,
            Cnpj = lead.Cnpj,
            RazaoSocial = lead.RazaoSocial,
            Cep = lead.Cep,
            Endereco = lead.Logradouro,
            Bairro = lead.Bairro,
            Cidade = lead.Cidade,
            Estado = lead.Estado,
            Numero = lead.Numero,
            Complemento = lead.Complemento,
            Revision = lead.RowVersion
        };

    public static LeadDto MapToDto(this LeadData lead)
        => new(
            lead.Id,
            lead.RazaoSocial,
            lead.Cnpj,
            lead.Endereco,
            lead.Numero,
            lead.Complemento,
            lead.Cep,
            lead.Cidade,
            lead.Bairro,
            lead.Estado,            
            lead.Revision
        );
}
