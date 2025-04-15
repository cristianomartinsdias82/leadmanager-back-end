using Application.AddressSearch.Models;
using ViaCep.ServiceClient.Models;

namespace ViaCep.ServiceClient.Mappings;

internal static class AddressMapper
{
	public static Address ToAddress(this Endereco endereco)
		=> new()
		{
			Bairro = endereco.Bairro,
			Cep = endereco.Cep,
			Localidade = endereco.Localidade,
			Logradouro = endereco.Logradouro,
			Uf = endereco.Uf
		};
}
