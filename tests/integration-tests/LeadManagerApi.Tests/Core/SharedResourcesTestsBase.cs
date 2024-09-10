using LeadManagerApi.Tests.Core.Factories;
using Xunit;

namespace LeadManagerApi.Tests.Core;

[Collection(SharedTestCollection.TestCollection)]
public abstract class SharedResourcesTestsBase : IAsyncLifetime
{
    protected const string LeadsEndpoint = "/api/leads";
    protected const string AddressesEndpoint = "/api/addresses";
    protected const string UserLoginAuditingEndpoint = "/api/auditing/logins/log";

    protected readonly LeadManagerWebApplicationFactory _factory;

    public SharedResourcesTestsBase(LeadManagerWebApplicationFactory factory)
    {
        _factory = factory;
    }

    public virtual Task InitializeAsync()
        => Task.CompletedTask;

    public virtual async Task DisposeAsync()
        => await _factory.ResetDatabaseAsync();
}
