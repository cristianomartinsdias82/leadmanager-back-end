using ViaCep.ServiceClient.Models;

namespace Application.Features.Addresses.Queries.SearchAddressByZipCode;

public sealed record SearchAddressByZipCodeQueryResponse
{
    private SearchAddressByZipCodeQueryResponse() { }

    public required string Endereco { get; init; }
    public required string Bairro { get; init; }
    public required string Cidade { get; init; }
    public required string Estado { get; init; }
    public required string Cep { get; init; }

    public static SearchAddressByZipCodeQueryResponse FromModel(Endereco endereco)
        => new SearchAddressByZipCodeQueryResponse
        {
            Endereco = endereco.Logradouro,
            Bairro = endereco.Bairro,
            Cidade = endereco.Localidade,
            Estado = endereco.Uf,
            Cep = endereco.Cep
        };
}