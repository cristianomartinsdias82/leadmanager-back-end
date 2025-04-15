using System.Text.Json.Serialization;

namespace Application.AddressSearch.Models;

public record Address
{
    [JsonPropertyName("logradouro")]
    public required string Logradouro { get; init; }
    [JsonPropertyName("bairro")]
    public required string Bairro { get; init; }
    [JsonPropertyName("localidade")]
    public required string Localidade { get; init; }
    [JsonPropertyName("uf")]
    public required string Uf { get; init; }
    [JsonPropertyName("cep")]
    public required string Cep { get; init; }
    [JsonPropertyName("erro")]
    public bool? Erro { get; set; }
}
