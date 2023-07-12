using MediatR;
using Shared.Results;

namespace Application.Features.Leads.Commands.UpdateLead;

public sealed class UpdateLeadCommandRequest : IRequest<ApplicationResponse<UpdateLeadCommandResponse>>
{
    public Guid? Id { get; set; }
    public string? Cnpj { get; set; }
    public string? RazaoSocial { get; set; }
    public string? Cep { get; set; }
    public string? Endereco { get; set; }
    public string? Bairro { get; set; }
    public string? Cidade { get; set; }
    public string? Estado { get; set; }
    public string? Complemento { get; set; }
    public string? Numero { get; set; }
}
