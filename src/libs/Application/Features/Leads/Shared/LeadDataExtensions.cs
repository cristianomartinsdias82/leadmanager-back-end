using Core.Entities;
using CrossCutting.MessageContracts;

namespace Application.Features.Leads.Shared;

public static class LeadExtensions
{
    public static List<LeadDto> AsDtoList(this IEnumerable<LeadData> leads)
        => leads?
        .Select(AsDto)
        .ToList() ?? new List<LeadDto>();

    public static LeadDto AsDto(this LeadData lead)
        => new()
        {
            Id = lead.Id,
            Cnpj = lead.Cnpj,
            RazaoSocial = lead.RazaoSocial,
            Cep = lead.Cep,
            Endereco = lead.Endereco,
            Bairro = lead.Bairro,
            Cidade = lead.Cidade,
            Estado = lead.Estado,
            Numero = lead.Numero,
            Complemento = lead.Complemento
        };

    public static List<LeadDto> AsDtoList(this IEnumerable<Lead> leads)
        => leads?
        .Select(AsDto)
        .ToList() ?? new List<LeadDto>();

    public static LeadDto AsDto(this Lead lead)
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
