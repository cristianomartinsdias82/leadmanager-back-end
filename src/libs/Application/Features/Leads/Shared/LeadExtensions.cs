using Core.Entities;

namespace Application.Features.Leads.Shared
{
    public static class LeadExtensions
    {
        public static List<LeadDto> ToDtoList(this IEnumerable<Lead> leads)
            => leads?
            .Select(ToDto)
            .ToList() ?? new List<LeadDto>();

        public static LeadDto ToDto(this Lead lead)
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
}
