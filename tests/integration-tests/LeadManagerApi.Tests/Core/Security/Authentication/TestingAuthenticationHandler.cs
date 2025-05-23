﻿using Application.Security;
using IdentityModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;
using static Application.Security.LeadManagerSecurityConfiguration;

namespace LeadManagerApi.Tests.Core.Security.Authentication;

internal class TestingAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public const string TestingScheme = nameof(TestingScheme);

    public TestingAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory loggerFactory,
        UrlEncoder urlEncoder) : base(options, loggerFactory, urlEncoder) { }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var identity = new ClaimsIdentity(
            [
                new Claim (LeadManagerSecurityConfiguration.ClaimTypes.LDM, LeadManagerSecurityConfiguration.Claims.Read),
                new Claim (LeadManagerSecurityConfiguration.ClaimTypes.LDM, LeadManagerSecurityConfiguration.Claims.Insert),
                new Claim (LeadManagerSecurityConfiguration.ClaimTypes.LDM, LeadManagerSecurityConfiguration.Claims.BulkInsert),
                new Claim (LeadManagerSecurityConfiguration.ClaimTypes.LDM, LeadManagerSecurityConfiguration.Claims.Update),
                new Claim (LeadManagerSecurityConfiguration.ClaimTypes.LDM, LeadManagerSecurityConfiguration.Claims.Delete),
                new Claim (JwtClaimTypes.Subject, "955FBD5F-7479-440F-B581-799119060AED"),
                new Claim (JwtClaimTypes.Email, "test-user@leadmanager.com.br"),
                new Claim (System.Security.Claims.ClaimTypes.Name, "Testing"),
                new Claim (System.Security.Claims.ClaimTypes.Role, Roles.Administrators),
            ],
			TestingScheme);
		var principal = new ClaimsPrincipal(identity);

        var ticket = new AuthenticationTicket(principal, TestingScheme);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
