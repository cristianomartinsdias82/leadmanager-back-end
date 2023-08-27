using Core.Entities;

namespace CrossCutting.MessageContracts;

public static class LeadExtensions
{
    public static List<LeadData> AsMessageContractList(this IEnumerable<Lead> leads)
            => leads?
            .Select(AsMessageContract)
            .ToList() ?? new List<LeadData>();

    public static LeadData AsMessageContract(this Lead lead)
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
            Complemento = lead.Complemento
        };
}
