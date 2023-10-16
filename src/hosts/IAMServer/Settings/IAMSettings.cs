using IdentityServer4.Models;

namespace IAMServer.Settings;

public class IAMSettings
{
    public IReadOnlyCollection<ApiScope> ApiScopes { get; init; } = Array.Empty<ApiScope>();
    
    public IReadOnlyCollection<ApiResource> ApiResources { get; init; } = default!;

    public IReadOnlyCollection<Client> Clients { get; init; } = default!;

    public IReadOnlyCollection<IdentityResource> IdentityResources
        => new IdentityResource[]
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
            new IdentityResource("roles", new string[]{ "role" }),
        };
}
