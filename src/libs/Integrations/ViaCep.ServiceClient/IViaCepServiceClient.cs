using ViaCep.ServiceClient.Models;

namespace ViaCep.ServiceClient;

public interface IViaCepServiceClient
{
    Task<Endereco?> SearchAsync(string cep, CancellationToken cancellationToken);
}