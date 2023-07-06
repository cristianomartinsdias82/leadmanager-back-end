using Core.Entities;

namespace Tests.Common.ObjectMothers.Core;

public class LeadMother
{
    private LeadMother() { }

    public static Lead XptoLLC()
        => new Lead(
                "53.302.821/0001-49",
                "Xpto LLC",
                "01234-567",
                "Rua das Pitombeiras",
                "Vila Alexandria",
                "São Paulo",
                "SP",
                "287",
                default);
}