using ViaCep.ServiceClient.Models;

namespace Tests.Common.ObjectMothers.Integrations.ViaCep;

public class AddressMother
{
    private AddressMother() { }
    public static Endereco FullAddress()
    {
        return new()
        {
            Cep = "01234-567",
            Bairro = "Vila Alexandria",
            Localidade = "São Paulo",
            Logradouro = "Rua das Pitombeiras",
            Uf = "SP"
        };
    }

    public static Endereco NotFoundAddress()
    {
        return new()
        {
            Cep = string.Empty,
            Bairro = string.Empty,
            Localidade = string.Empty,
            Logradouro = string.Empty,
            Uf = string.Empty
        };
    }

    public static Endereco FaultedAddress()
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
