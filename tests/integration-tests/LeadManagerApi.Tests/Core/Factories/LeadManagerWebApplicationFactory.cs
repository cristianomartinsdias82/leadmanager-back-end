using Application.Core.Contracts.Persistence;
using Application.Core.Contracts.Repository.Caching;
using Application.Core.Contracts.Repository.Prospecting;
using Application.Core.Contracts.Repository.UnitOfWork;
using CrossCutting.Caching;
using CrossCutting.Caching.Redis;
using CrossCutting.Caching.Redis.Configuration;
using CrossCutting.Messaging.RabbitMq;
using CrossCutting.Security.IAM;
using DotNet.Testcontainers.Builders;
using Infrastructure.Persistence;
using Infrastructure.Repository.Caching;
using Infrastructure.Repository.Prospecting;
using Infrastructure.Repository.UnitOfWork;
using LeadManagerApi.Core.Configuration;
using LeadManagerApi.Tests.Core.Security.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using Respawn;
using RichardSzalay.MockHttp;
using Shared.Settings;
using System.Data.Common;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Testcontainers.MsSql;
using Testcontainers.Redis;
using Tests.Common.ObjectMothers.Domain;
using Tests.Common.ObjectMothers.Integrations.ViaCep;
using ViaCep.ServiceClient;
using ViaCep.ServiceClient.Configuration;
using ViaCep.ServiceClient.Models;
using Xunit;
using static Application.Security.LeadManagerSecurityConfiguration;

namespace LeadManagerApi.Tests.Core.Factories;

public class LeadManagerWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly MockHttpMessageHandler _httpHandlerMock = new MockHttpMessageHandler();
    private readonly MsSqlContainer _dbContainer;
    private readonly RedisContainer _cacheContainer;
    private JsonSerializerOptions _jsonSerializerOptions = default!;
    private DbConnection _dbConnection = default!;
    private Respawner _respawner = default!;

    public IConfiguration Configuration { get; private set; } = default!;

    public LeadManagerWebApplicationFactory()
    {
        Configuration = Services.GetRequiredService<IConfiguration>();

        var dbResourcePortNumber = Configuration["DataSourceSettings:PortNumber"]!;
        _dbContainer = new MsSqlBuilder()
                                    //.WithHostname("localhost") //These parameters are interesting in case you wish to inspect
                                    //.WithPortBinding(dbResourcePortNumber, false) //the generated database in some debugging session
                                    //.WithPassword("Y0urStr0nGP@sswoRD_2023") //so you can connect by using some db client tool
                                    .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(int.Parse(dbResourcePortNumber)))
                                    .Build();

        var cacheResourcePortNumber = Configuration["RedisCacheProviderSettings:PortNumber"]!;
        var cacheImageName = Configuration["RedisCacheProviderSettings:ImageName"]!;
        _cacheContainer = new RedisBuilder()
                                    .WithImage(cacheImageName)
                                    .WithPortBinding(cacheResourcePortNumber, false) //the generated database in some debugging session
                                    .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(int.Parse(cacheResourcePortNumber)))
                                    .Build();
    }

    public async Task InitializeAsync()
    {
        _jsonSerializerOptions = Services.GetRequiredService<IOptions<JsonOptions>>().Value.JsonSerializerOptions;

        await _dbContainer.StartAsync();
        _dbConnection = new SqlConnection(_dbContainer.GetConnectionString());

        await _cacheContainer.StartAsync();

        await InitializeRespawnerAsync();
    }

    public async Task ResetDatabaseAsync()
    {
        await _respawner.ResetAsync(_dbConnection);
    }

    public new async Task DisposeAsync()
    {
        await _dbContainer.StopAsync();
        await _cacheContainer.StopAsync();

        await base.DisposeAsync();
    }

    //public virtual HttpClient CreateHttpClientMock(
    //    Func<MockHttpMessageHandler, MockHttpMessageHandler> factory,
    //    bool includeApiKeyHeader = true)
    //{
    //    var handler = factory(_httpHandlerMock);

    //    var httpClient = handler.ToHttpClient();

    //    if (includeApiKeyHeader)
    //    {
    //        var apiSettings = Configuration
    //                            .GetSection(nameof(LeadManagerApiSettings))
    //                            .Get<LeadManagerApiSettings>()!;

    //        httpClient.DefaultRequestHeaders.Add(apiSettings.ApiKeyRequestHeaderName,
    //                                             apiSettings.ApiKeyRequestHeaderValue);
    //    }

    //    return httpClient;
    //}

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

    //public async Task UsingContextAsync(Func<ILeadManagerDbContext, Task> action)
    //{
    //    await using var scope = Services.CreateAsyncScope();
    //    await using var context = scope.ServiceProvider.GetRequiredService<ILeadManagerDbContext>();

    //    await action(context);
    //}

    //public async Task<TResult> UsingContextAsync<TResult>(Func<ILeadManagerDbContext, Task<TResult>> action)
    //{
    //    await using var scope = Services.CreateAsyncScope();
    //    await using var context = scope.ServiceProvider.GetRequiredService<ILeadManagerDbContext>();

    //    return await action(context);
    //}

    public T? DeserializeFromJson<T>(string json)
        => JsonSerializer.Deserialize<T>(json, _jsonSerializerOptions);

    private async Task InitializeRespawnerAsync()
    {
        await _dbConnection.OpenAsync();

        _respawner = await Respawner.CreateAsync(
            _dbConnection,
            new RespawnerOptions
            {
                DbAdapter = DbAdapter.SqlServer,
                //WithReseed = true
                //SchemasToInclude = new[] { "LeadManager" }
            });
    }

    private async Task Seed(ILeadManagerDbContext context)
    {
        if (!context.Leads.Any())
        {
            await context.Leads.AddRangeAsync(LeadMother.Leads());
            await context.SaveChangesAsync();
        }
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder
        .UseEnvironment("IntegrationTests")
        .ConfigureTestServices(services =>
        {
            services.AddSingleton(services => Configuration.GetSection(nameof(LeadManagerApiSettings)).Get<LeadManagerApiSettings>()!);

            services.RemoveAll<IHttpContextAccessor>();
            services.AddHttpContextAccessor();

            services.RemoveAll<IUserService>();
            services.AddScoped<IUserService, UserService>();

            services.AddAuthentication(defaultScheme: TestingAuthenticationHandler.TestingScheme)
                        .AddScheme<AuthenticationSchemeOptions, TestingAuthenticationHandler>(
                                        TestingAuthenticationHandler.TestingScheme,
                                        options => { });
            services.AddAuthorization(policyOptions =>
            {
                policyOptions.AddPolicy(Policies.LeadManagerDefaultPolicy, policy =>
                {
                    //policy.AddAuthenticationSchemes(TestingAuthenticationHandler.TestingScheme);
                    //policy.RequireAuthenticatedUser();
                    policy.RequireClaim(
                        ClaimTypes.LDM,
                        Permissions.Read,
                        Permissions.BulkInsert,
                        Permissions.Insert,
                        Permissions.Update,
                        Permissions.Delete);
                });

                policyOptions.AddPolicy(Policies.LeadManagerRemovePolicy, policy =>
                {
                    //policy.AddAuthenticationSchemes(TestingAuthenticationHandler.TestingScheme);
                    //policy.RequireAuthenticatedUser();
                    policy.RequireRole(Roles.Administrators);
                    policy.RequireClaim(ClaimTypes.LDM, Permissions.Delete);
                });
            });

            services.AddSingleton(services => Configuration.GetSection(nameof(DataSourceSettings)).Get<DataSourceSettings>()!);
                
            services.RemoveAll(typeof(DbContextOptions<LeadManagerDbContext>));
            services.RemoveAll<ILeadManagerDbContext>();
            services.AddScoped<ILeadManagerDbContext>(provider =>
            {
                var dataSourceSettings = Services.GetRequiredService<DataSourceSettings>();
                var optionsBuilder = new DbContextOptionsBuilder<LeadManagerDbContext>();

                var options = optionsBuilder
                    //.UseSqlServer(_dbContainer.GetConnectionString())
                    .UseSqlServer(_dbConnection)
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

                Seed(context).GetAwaiter().GetResult();

                return context;
            });

            services.AddSingleton(services => Configuration.GetSection($"{nameof(CachingPoliciesSettings)}:{nameof(LeadsCachingPolicy)}").Get<LeadsCachingPolicy>()!);
            services.AddSingleton(services => Configuration.GetSection($"{nameof(CachingPoliciesSettings)}:{nameof(AddressesCachingPolicy)}").Get<AddressesCachingPolicy>()!);
            services.RemoveAll<ICacheProvider>();
            services.AddSingleton(services => Configuration.GetSection(nameof(RedisCacheProviderSettings)).Get<RedisCacheProviderSettings>()!);
            services.AddStackExchangeRedisCache(options =>
            {
                var cacheProviderSettings = Services.GetRequiredService<RedisCacheProviderSettings>();

                options.Configuration = $"{cacheProviderSettings.Server}:{cacheProviderSettings.PortNumber}";
            });
            services.TryAddScoped<ICacheProvider, RedisCacheProvider>();

            services.RemoveAll<IUnitOfWork>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.RemoveAll<ILeadRepository>();
            services.AddScoped<ILeadRepository, LeadRepository>();
            services.Decorate<ILeadRepository, CachingLeadRepository>();

            services.RemoveAll<ICachingLeadRepository>();
            services.AddScoped<ICachingLeadRepository, CachingLeadRepository>();

            services.RemoveAll<IRabbitMqChannelFactory>();
            services.AddSingleton(services => Substitute.For<IRabbitMqChannelFactory>());

            services.AddSingleton(services => Configuration.GetSection("ServiceIntegrations:ViaCep").Get<ViaCepIntegrationSettings>()!);
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
        });
    }
}