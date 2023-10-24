using LeadManagerApi.Core.Configuration.Security;
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
                new Claim (LeadManagerApiSecurityConfiguration.ClaimTypes.LDM, LeadManagerApiSecurityConfiguration.Claims.Read),
                new Claim (LeadManagerApiSecurityConfiguration.ClaimTypes.LDM, LeadManagerApiSecurityConfiguration.Claims.Insert),
                new Claim (LeadManagerApiSecurityConfiguration.ClaimTypes.LDM, LeadManagerApiSecurityConfiguration.Claims.BulkInsert),
                new Claim (LeadManagerApiSecurityConfiguration.ClaimTypes.LDM, LeadManagerApiSecurityConfiguration.Claims.Update),
                new Claim (LeadManagerApiSecurityConfiguration.ClaimTypes.LDM, LeadManagerApiSecurityConfiguration.Claims.Delete)
            },
            "Testing");
        var principal = new ClaimsPrincipal(identity);

        var ticket = new AuthenticationTicket(principal, TestingScheme);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
