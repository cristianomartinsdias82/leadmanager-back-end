using Application.Contracts.Persistence;
using Application.Features.Leads.Shared;
using CrossCutting.Caching;
using CrossCutting.MessageContracts;
using CrossCutting.Messaging.RabbitMq;
using CrossCutting.Security.IAM;
using Infrastructure.Persistence;
using LeadManagerApi.Core.Configuration;
using LeadManagerApi.Tests.Common.Security.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using RichardSzalay.MockHttp;
using System.Data.Common;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Tests.Common.ObjectMothers.Core;
using Tests.Common.ObjectMothers.Integrations.ViaCep;
using ViaCep.ServiceClient;
using ViaCep.ServiceClient.Configuration;
using ViaCep.ServiceClient.Models;
using Xunit;

namespace LeadManagerApi.Tests.Common.Factories;

public class LeadManagerWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    public const string LeadsEndpoint = "/api/leads";
    public const string AddressesEndpoint = "/api/addresses";

    public IConfiguration Configuration { get; private set; } = default!;
    private JsonSerializerOptions _jsonSerializerOptions = default!;

    private readonly MockHttpMessageHandler _httpHandlerMock = new MockHttpMessageHandler();

    public Task InitializeAsync()
    {
        Configuration = Services.GetRequiredService<IConfiguration>();
        _jsonSerializerOptions = Services.GetRequiredService<IOptions<JsonOptions>>().Value.JsonSerializerOptions;

        return Task.CompletedTask;
    }

    public virtual HttpClient CreateHttpClientMock(
        Func<MockHttpMessageHandler, MockHttpMessageHandler> factory,
        bool includeApiKeyHeader = true)
    {
        var handler = factory(_httpHandlerMock);

        var httpClient = handler.ToHttpClient();

        if (includeApiKeyHeader)
        {
            var apiSettings = Configuration
                                .GetSection(nameof(LeadManagerApiSettings))
                                .Get<LeadManagerApiSettings>()!;

            httpClient.DefaultRequestHeaders.Add(apiSettings.ApiKeyRequestHeaderName,
                                                 apiSettings.ApiKeyRequestHeaderValue);
        }

        return httpClient;
    }

    public virtual HttpClient CreateHttpClient(bool includeApiKeyHeader = true)
    {
        var apiSettings = Services.GetRequiredService<LeadManagerApiSettings>();
        var headers = includeApiKeyHeader ? new (string Name, string Value)[] { (apiSettings.ApiKeyRequestHeaderName!, apiSettings.ApiKeyRequestHeaderValue) } : Enumerable.Empty<(string Name, string Value)>();
            
        return CreateHttpClient(headers.ToArray());
    }

    public virtual HttpClient CreateHttpClient(params (string Name, string Value)[] headers)
    {
        var httpClient = CreateClient();

        foreach (var header in headers)
            httpClient.DefaultRequestHeaders.Add(
                header.Name,
                header.Value);

        return httpClient;
    }

    public async Task UsingContextAsync(Func<ILeadManagerDbContext, Task> action)
    {
        await using var scope = Services.CreateAsyncScope();
        await using var context = scope.ServiceProvider.GetRequiredService<ILeadManagerDbContext>();

        await action(context);
    }

    public async Task<TResult> UsingContextAsync<TResult>(Func<ILeadManagerDbContext, Task<TResult>> action)
    {
        await using var scope = Services.CreateAsyncScope();
        await using var context = scope.ServiceProvider.GetRequiredService<ILeadManagerDbContext>();

        return await action(context);
    }

    public T? DeserializeFromJson<T>(string json)
        => JsonSerializer.Deserialize<T>(json, _jsonSerializerOptions);

    public new async Task DisposeAsync()
        => await base.DisposeAsync();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder
            .UseEnvironment("IntegrationTests")
            .ConfigureTestServices(services =>
            {
                services.RemoveAll<IHttpContextAccessor>();
                services.AddHttpContextAccessor();

                services.RemoveAll<IUserService>();
                services.AddScoped<IUserService, UserService>();

                services.RemoveAll<DbConnection>();
                services.AddSingleton<DbConnection>(_ =>
                {
                    var connection = new SqliteConnection(Configuration["DataSourceSettings:ConnectionString"]);
                    connection.Open();

                    return connection;
                });

                services.AddAuthentication(defaultScheme: TestingAuthenticationHandler.TestingScheme)
                            .AddScheme<AuthenticationSchemeOptions, TestingAuthenticationHandler>(
                                            TestingAuthenticationHandler.TestingScheme,
                                            options => { });

                services.RemoveAll(typeof(DbContextOptions<LeadManagerDbContext>));
                services.RemoveAll<ILeadManagerDbContext>();
                services.AddScoped<ILeadManagerDbContext>(provider =>
                {
                    var connection = provider.GetRequiredService<DbConnection>();

                    var optionsBuilder = new DbContextOptionsBuilder<LeadManagerDbContext>();

                    var options = optionsBuilder
                        .UseSqlite(connection)
                        .EnableDetailedErrors()
                        .EnableSensitiveDataLogging()
                        .LogTo(
                            action: Console.WriteLine,
                            categories: new[] { DbLoggerCategory.Database.Command.Name },
                            minimumLevel: LogLevel.Information)
                        .Options;

                    var context = new LeadManagerDbContext(
                                        options,
                                        provider.GetRequiredService<IUserService>());

                    context.Database.EnsureCreated();

                    return context;
                });                

                services.AddSingleton(factory => Configuration.GetSection("ServiceIntegrations:ViaCep").Get<ViaCepIntegrationSettings>()!);

                services.AddSingleton(factory => Configuration.GetSection(nameof(LeadManagerApiSettings)).Get<LeadManagerApiSettings>()!);

                services.RemoveAll<IViaCepServiceClient>();
                services.AddScoped<IViaCepServiceClient>(services =>
                {
                    var leadManagerApiSettings = Services.GetRequiredService<LeadManagerApiSettings>();
                    var viaCepApiSettings = Services.GetRequiredService<ViaCepIntegrationSettings>();

                    var httpContextAccessor = services.GetRequiredService<IHttpContextAccessor>();
                    if (!httpContextAccessor.HttpContext!.Request.Query.TryGetValue("cep", out var cep))
                        cep = Endereco.CepInvalido;

                    _httpHandlerMock
                        .When($"{viaCepApiSettings.Uri}/{string.Format(viaCepApiSettings.Endpoint, cep!)}")
                        .Respond(
                            HttpStatusCode.OK,
                            JsonContent.Create(cep.Equals(Endereco.CepInvalido) ? AddressMother.NotFoundAddress() : AddressMother.FullAddress())
                        );

                    var httpClient = _httpHandlerMock.ToHttpClient();
                    httpClient.BaseAddress = new Uri(viaCepApiSettings.Uri);
                    httpClient.Timeout = TimeSpan.FromSeconds(viaCepApiSettings.RequestTimeoutInSeconds);
                    httpClient.DefaultRequestHeaders.Add(
                        leadManagerApiSettings.ApiKeyRequestHeaderName,
                        leadManagerApiSettings.ApiKeyRequestHeaderValue);

                    return new ViaCepServiceClient(httpClient, viaCepApiSettings, default!);
                });

                services.RemoveAll<ICacheProvider>();
                services.AddScoped(services =>
                {
                    var cacheProviderMock = Substitute.For<ICacheProvider>();
                    cacheProviderMock.GetAsync<IEnumerable<LeadData>>(
                                        Arg.Any<string>(),
                                        Arg.Any<CancellationToken>())
                                    .Returns(LeadMother.Leads().MapToMessageContractList());

                    return cacheProviderMock;
                });

                services.RemoveAll<IRabbitMqChannelFactory>();
                services.AddSingleton(services => Substitute.For<IRabbitMqChannelFactory>());
            });
    }
}
