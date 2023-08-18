using Core.Entities;

namespace Tests.Common.ObjectMothers.Core;

public class LeadMother
{
    private LeadMother() { }

    public static Lead XptoLLC()
        => new Lead(
                CnpjMother.MaskedWellformedValidOne(),
                "Xpto LLC",
                "04858-040",
                "Rua das Pitombeiras",
                "Vila Alexandria",
                "São Paulo",
                "SP",
                "287",
                default);

    public static Lead GumperInc()
        => new Lead(
                CnpjMother.MaskedWellformedValidOne(),
                "Gumper Inc.",
                "04661-100",
                "Rua das Palmeiras",
                "Vila Zambunitte",
                "São Paulo",
                "SP",
                default,
                default);

    public static List<Lead> Leads()
    =>  new List<Lead>()
        {
            GumperInc(),
            XptoLLC()
        };
}