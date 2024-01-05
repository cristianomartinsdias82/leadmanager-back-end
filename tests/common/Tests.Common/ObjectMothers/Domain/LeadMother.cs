using Domain.Prospecting.Entities;

namespace Tests.Common.ObjectMothers.Domain;

public class LeadMother
{
    private LeadMother() { }

    public static Lead XptoLLC()
        => Lead.Criar(
                CnpjMother.WellformedCnpjs.ElementAt(0),
                "Xpto LLC",
                "04858-040",
                "Rua das Pitombeiras",
                "Vila Alexandria",
                "São Paulo",
                "SP",
                "287",
                default);

    public static Lead GumperInc()
        => Lead.Criar(
                CnpjMother.WellformedCnpjs.ElementAt(1),
                "Gumper Inc.",
                "04661-100",
                "Rua das Palmeiras",
                "Vila Zambunitte",
                "São Paulo",
                "SP",
                default,
                default);

    public static IEnumerable<Lead> Leads()
    {
        yield return GumperInc();
        yield return XptoLLC();
    }
}