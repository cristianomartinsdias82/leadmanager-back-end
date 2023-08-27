namespace Application.Features.Leads.Shared;

public sealed record LeadDto
{
    public Guid Id { get; set; }

    public string Cnpj { get; set; } = default!;

    public string RazaoSocial { get; set; } = default!;

    public string Cep { get; set; } = default!;

    public string Endereco { get; set; } = default!;

    public string Bairro { get; set; } = default!;

    public string Cidade { get; set; } = default!;

    public string Estado { get; set; } = default!;

    public string? Numero { get; set; }

    public string? Complemento { get; set; }
}