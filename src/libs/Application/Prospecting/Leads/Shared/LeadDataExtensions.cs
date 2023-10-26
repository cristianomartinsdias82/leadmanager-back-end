using Domain.Prospecting.Entities;
using CrossCutting.MessageContracts;

namespace Application.Prospecting.Leads.Shared;

public static class LeadExtensions
{
    public static List<LeadDto> MapToDtoList(this IEnumerable<Lead> leads)
        => leads?
        .Select(ld => ld.MapToDto())
        .ToList() ?? new List<LeadDto>();

    public static List<LeadData> MapToMessageContractList(this IEnumerable<LeadDto> leads)
        => leads?
        .Select(MapToMessageContract)
        .ToList() ?? new List<LeadData>();

    public static LeadData MapToMessageContract(this LeadDto lead)
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
            Complemento = lead.Complemento,
            Revision = lead.Revision
        };

    public static Lead MapToEntity(this LeadData leadData)
        => Lead.MapFromDto(leadData.MapToDto());
}
