using Application.Security;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace LeadManagerApi.Tests.Core.Security.Authentication;

internal class TestingAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public const string TestingScheme = nameof(TestingScheme);

    public TestingAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory loggerFactory,
        UrlEncoder urlEncoder,
        ISystemClock systemClock) : base(options, loggerFactory, urlEncoder, systemClock) { }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var identity = new ClaimsIdentity(
            new Claim[]
            {
                new Claim (LeadManagerSecurityConfiguration.ClaimTypes.LDM, LeadManagerSecurityConfiguration.Claims.Read),
                new Claim (LeadManagerSecurityConfiguration.ClaimTypes.LDM, LeadManagerSecurityConfiguration.Claims.Insert),
                new Claim (LeadManagerSecurityConfiguration.ClaimTypes.LDM, LeadManagerSecurityConfiguration.Claims.BulkInsert),
                new Claim (LeadManagerSecurityConfiguration.ClaimTypes.LDM, LeadManagerSecurityConfiguration.Claims.Update),
                new Claim (LeadManagerSecurityConfiguration.ClaimTypes.LDM, LeadManagerSecurityConfiguration.Claims.Delete)
            },
            "Testing");
        var principal = new ClaimsPrincipal(identity);

        var ticket = new AuthenticationTicket(principal, TestingScheme);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
