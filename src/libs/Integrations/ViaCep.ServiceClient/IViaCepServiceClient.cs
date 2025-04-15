using Application.AddressSearch.Contracts;
//using ViaCep.ServiceClient.Models;

namespace ViaCep.ServiceClient;

internal interface IViaCepServiceClient : IAddressSearch
{
	//Task<Endereco?> SearchAsync(string cep, CancellationToken cancellationToken)
}