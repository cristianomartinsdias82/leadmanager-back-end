using LeadManagerApi.Tests.Core.Factories;
using Xunit;

namespace LeadManagerApi.Tests.Core;

[CollectionDefinition(TestCollection)]
public class SharedTestCollection : ICollectionFixture<LeadManagerWebApplicationFactory>
{
    public const string TestCollection = nameof(TestCollection);
}
