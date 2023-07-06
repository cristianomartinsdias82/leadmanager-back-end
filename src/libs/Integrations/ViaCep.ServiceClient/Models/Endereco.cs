using System.Text.Json.Serialization;

namespace ViaCep.ServiceClient.Models
{
    public record struct Endereco
    {
        public static string CepInvalido = "00000000";

        [JsonPropertyName("logradouro")]
        public string Logradouro { get; init; }
        [JsonPropertyName("bairro")]
        public string Bairro { get; init; }
        [JsonPropertyName("localidade")]
        public string Localidade { get; init; }
        [JsonPropertyName("uf")]
        public string Uf { get; init; }
        [JsonPropertyName("cep")]
        public string Cep { get; init; }
        [JsonPropertyName("erro")]
        public bool? Erro { get; set; }
    }
}
