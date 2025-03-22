using System.Collections;
using Tests.Common.ObjectMothers.Domain;

namespace Tests.Common.ObjectMothers.Application;

//Used in conjunction with ClassData attribute in Unit tests classes
public sealed class LeadSearchMatchingTermsExemplars : IEnumerable<object[]>
{
	public IEnumerator<object[]> GetEnumerator()
	{
		yield return new object[] { LeadMother.GumperInc().RazaoSocial };
		yield return new object[] { LeadMother.XptoLLC().RazaoSocial };
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}
}
