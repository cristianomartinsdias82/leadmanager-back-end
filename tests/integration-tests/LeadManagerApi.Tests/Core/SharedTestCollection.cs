using LeadManagerApi.Tests.Core.Factories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace LeadManagerApi.Tests.Core;

[CollectionDefinition(TestCollection)]
public class SharedTestCollection : ICollectionFixture<LeadManagerWebApplicationFactory>
{
    public const string TestCollection = nameof(TestCollection);

}
