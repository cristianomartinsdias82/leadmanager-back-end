using ProtoBuf;

namespace CrossCutting.MessageContracts;

[ProtoContract]
public struct LeadData
{
    public LeadData() { }

    [ProtoMember(01)]
    public Guid Id { get; set; }

    [ProtoMember(02)]
    public string Cnpj { get; set; } = default!;

    [ProtoMember(03)]
    public string RazaoSocial { get; set; } = default!;

    [ProtoMember(04)]
    public string Cep { get; set; } = default!;

    [ProtoMember(05)]
    public string Endereco { get; set; } = default!;

    [ProtoMember(06)]
    public string Bairro { get; set; } = default!;

    [ProtoMember(07)]
    public string Cidade { get; set; } = default!;

    [ProtoMember(08)]
    public string Estado { get; set; } = default!;

    [ProtoMember(09)]
    public string? Numero { get; set; }

    [ProtoMember(10)]
    public string? Complemento { get; set; }

    [ProtoMember(11)]
    public byte[] Revision { get; set; } = default!;

    public bool IsNull => string.IsNullOrWhiteSpace(Cnpj);
}