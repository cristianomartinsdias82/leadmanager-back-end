using Application.AddressSearch.Models;
//using ViaCep.ServiceClient.Models;

namespace Application.Prospecting.Addresses.Queries.SearchAddressByZipCode;

public sealed record SearchAddressByZipCodeQueryResponse
{
    public required string Endereco { get; init; }
    public required string Bairro { get; init; }
    public required string Cidade { get; init; }
    public required string Estado { get; init; }
    public required string Cep { get; init; }

	public static SearchAddressByZipCodeQueryResponse FromModel(Address address)
		=> new()
		{
			Endereco = address.Logradouro,
			Bairro = address.Bairro,
			Cidade = address.Localidade,
			Estado = address.Uf,
			Cep = address.Cep
		};

	//public static SearchAddressByZipCodeQueryResponse FromModel(Endereco endereco)
	//    => new SearchAddressByZipCodeQueryResponse
	//    {
	//        Endereco = endereco.Logradouro,
	//        Bairro = endereco.Bairro,
	//        Cidade = endereco.Localidade,
	//        Estado = endereco.Uf,
	//        Cep = endereco.Cep
	//    };
}