using System.Collections;

namespace Tests.Common.ObjectMothers.Application;

//Used in conjunction with ClassData attribute in Unit tests classes
public sealed class LeadSearchNonMatchingTermsExemplars : IEnumerable<object[]>
{
	public IEnumerator<object[]> GetEnumerator()
	{
		yield return new object[] { "this be the good old days" };
		yield return new object[] { "le le le le" };
		yield return new object[] { "123.46878.24456" };
		yield return new object[] { "45654" };
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}
}
