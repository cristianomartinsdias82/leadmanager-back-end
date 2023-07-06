using MediatR;
using Shared.Results;

namespace Application.Features.Leads.Commands.UpdateLead;

public sealed class UpdateLeadCommandRequest : IRequest<ApplicationResponse<UpdateLeadCommandResponse>>
{
    public Guid? Id { get; set; }
    public required string Cnpj { get; init; }
    public required string RazaoSocial { get; init; }
    public required string Cep { get; init; }
    public required string Endereco { get; init; }
    public required string Bairro { get; init; }
    public required string Cidade { get; init; }
    public required string Estado { get; init; }
    public string? Complemento { get; init; }
    public string? Numero { get; init; }
}
