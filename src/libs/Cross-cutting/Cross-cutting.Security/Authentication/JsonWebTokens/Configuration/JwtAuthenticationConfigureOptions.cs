using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace CrossCutting.Security.Authentication.JsonWebTokens.Configuration;

internal class JwtAuthenticationConfigureOptions : IConfigureNamedOptions<JwtBearerOptions>
{
    private readonly IConfiguration _configuration;

    public JwtAuthenticationConfigureOptions(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void Configure(string? name, JwtBearerOptions options)
    {
        if (name == JwtBearerDefaults.AuthenticationScheme)
        {
            var settings = _configuration.GetSection(nameof(JwtAuthenticationProviderSettings))!.Get<JwtAuthenticationProviderSettings>()!;

            options.Authority = settings.AuthorityBaseUri;
            options.Audience = settings.Audience;
            options.MapInboundClaims = false;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                NameClaimType = "name",
                RoleClaimType = "role",
                ClockSkew = TimeSpan.FromSeconds(10), //https://www.youtube.com/watch?v=meBxWjA_2YY (The Auth Setting That Everyone MUST Change in .NET)
            };
        }
    }

    public void Configure(JwtBearerOptions options)
        => Configure(Options.DefaultName, options);
}
