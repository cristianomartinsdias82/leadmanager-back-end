using Application.AddressSearch.Models;
//using ViaCep.ServiceClient.Models;

namespace Tests.Common.ObjectMothers.Integrations.ViaCep;

public class AddressMother
{
    private AddressMother() { }
    public static Address FullAddress()
        => new()
        {
            Cep = "01234-567",
            Bairro = "Vila Alexandria",
            Localidade = "São Paulo",
            Logradouro = "Rua das Pitombeiras",
            Uf = "SP"
        };

    public static Address NotFoundAddress()
        => new()
        {
            Cep = string.Empty,
            Bairro = string.Empty,
            Localidade = string.Empty,
            Logradouro = string.Empty,
            Uf = string.Empty
        };

    public static Address FaultedAddress()
        => new()
        {
            Erro = true,
            Cep = default!,
            Bairro = default!,
            Localidade = default!,
            Logradouro = default!,
            Uf = default!
        };
}