﻿using Application.AddressSearch.Contracts;
using Application.Core.Contracts.Persistence;
using Application.Core.Contracts.Repository.Caching;
using Application.Core.Contracts.Repository.Prospecting;
using Application.Core.Contracts.Repository.UnitOfWork;
using CrossCutting.Caching;
using CrossCutting.Caching.Hybrid;
using CrossCutting.Caching.Redis;
using CrossCutting.Caching.Redis.Configuration;
using CrossCutting.Messaging.RabbitMq;
using CrossCutting.Security.IAM;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
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
//using RichardSzalay.MockHttp;
using Shared.Settings;
using System.Data.Common;
using System.Net.Http.Headers;
using System.Text.Json;
using Testcontainers.MsSql;
using Testcontainers.Redis;
using Tests.Common.ObjectMothers.DateTimeHandling;
using Tests.Common.ObjectMothers.Domain;
using ViaCep.ServiceClient;
using ViaCep.ServiceClient.Configuration;
using Xunit;
using static Application.Security.LeadManagerSecurityConfiguration;

namespace LeadManagerApi.Tests.Core.Factories;

public class LeadManagerWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
	//private readonly MockHttpMessageHandler _httpHandlerMock = new MockHttpMessageHandler(); //From RichardSzalay.MockHttp package. Works great, by the way.
	private readonly MsSqlContainer _dbContainer;
    private readonly RedisContainer _cacheContainer;
    private readonly IContainer _wireMockContainer;
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

        //Heads up! When running integration tests in your local machine, please make sure Redis server port number is not being used already (docker composition, for example)
        var cacheResourcePortNumber = Configuration["RedisCacheProviderSettings:PortNumber"]!;
        var cacheImageName = Configuration["RedisCacheProviderSettings:ImageName"]!;
        _cacheContainer = new RedisBuilder()
                                    .WithImage(cacheImageName)
                                    .WithPortBinding(cacheResourcePortNumber, false) //the generated database in some debugging session
                                    .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(int.Parse(cacheResourcePortNumber)))
                                    .Build();

		_wireMockContainer = new ContainerBuilder()
		                            .WithImage("wiremock/wiremock:3.7.0")
		                            .WithPortBinding(8080, true)
		                            .WithBindMount(Path.Combine(Environment.CurrentDirectory, "mocks"), "/home/wiremock", AccessMode.ReadWrite)
		                            .Build();
	}

	public async Task InitializeAsync()
    {
        _jsonSerializerOptions = Services.GetRequiredService<IOptions<JsonOptions>>().Value.JsonSerializerOptions;

        await _dbContainer.StartAsync();
        _dbConnection = new SqlConnection(_dbContainer.GetConnectionString());

        await _cacheContainer.StartAsync();

        await InitializeRespawnerAsync();

		await _wireMockContainer.StartAsync();
	}

    public async Task ResetDatabaseAsync()
    {
        try { await _respawner.ResetAsync(_dbConnection); }
        catch { }
    }

    public new async Task DisposeAsync()
    {
        await _dbContainer.StopAsync();

        await _cacheContainer.StopAsync();

		await _wireMockContainer.DisposeAsync();

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
        => CreateHttpClient([.. includeApiKeyHeader
                                ? [GetLeadManagerApiKeyNameValueHeader()]
                                : Enumerable.Empty<(string Name, string Value)>()]);

    public LeadManagerApiSettings GetLeadManagerApiSettings() =>
		Services.GetRequiredService<LeadManagerApiSettings>();

    public (string Name, string Value) GetLeadManagerApiKeyNameValueHeader()
    {
        var apiSettings = GetLeadManagerApiSettings();

        return (apiSettings.ApiKeyRequestHeaderName!, apiSettings.ApiKeyRequestHeaderValue);
    }

	public virtual HttpClient CreateHttpClient(params (string Name, string Value)[] headers)
    {
        var httpClient = CreateClient();

        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(TestingAuthenticationHandler.TestingScheme);

        foreach (var (name,value) in headers)
            httpClient.DefaultRequestHeaders.Add(name,value);

        return httpClient;
    }

	public virtual HttpClient CreateHttpClient(bool includeApiKeyHeader = true, params (string Name, string Value)[] headers)
	{
		var apiSettings = Services.GetRequiredService<LeadManagerApiSettings>();

		return CreateHttpClient([..headers, (apiSettings.ApiKeyRequestHeaderName!, apiSettings.ApiKeyRequestHeaderValue)]);
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

    private static async Task Seed(LeadManagerDbContext context)
    {
        if (!context.Leads.Any())
        {
            await context.Leads.AddRangeAsync(LeadMother.Leads());
            await context.LeadsFiles.AddRangeAsync(LeadsFileMother.LeadsFiles());
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

            services.AddAuthorizationBuilder()
				.AddPolicy(Policies.LeadManagerDefaultPolicy, policy =>
                {
                    policy.AddAuthenticationSchemes(TestingAuthenticationHandler.TestingScheme);
                    policy.RequireAuthenticatedUser();
                    policy.RequireClaim(
                        ClaimTypes.LDM,
                        Permissions.Read,
                        Permissions.BulkInsert,
                        Permissions.Insert,
                        Permissions.Update,
                        Permissions.Delete);
                })
				.AddPolicy(Policies.LeadManagerRemovePolicy, policy =>
                {
                    policy.AddAuthenticationSchemes(TestingAuthenticationHandler.TestingScheme);
                    policy.RequireAuthenticatedUser();
                    policy.RequireRole(Roles.Administrators);
                    policy.RequireClaim(ClaimTypes.LDM, Permissions.Delete);
                })
				.AddPolicy(Policies.LeadManagerAdministrativeTasksPolicy, policy =>
				{
					policy.AddAuthenticationSchemes(TestingAuthenticationHandler.TestingScheme);
					policy.RequireAuthenticatedUser();
					policy.RequireRole(Roles.Administrators);
				});

			services.AddSingleton(services => Configuration.GetSection(nameof(DataSourceSettings)).Get<DataSourceSettings>()!);
                
            services.RemoveAll<DbContextOptions<LeadManagerDbContext>>();
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
                        categories: [DbLoggerCategory.Database.Command.Name],
                        minimumLevel: LogLevel.Information)
                    .Options;

                var context = new LeadManagerDbContext(
                                    options,
                                    provider.GetRequiredService<IUserService>(),
									provider.GetRequiredService<TimeProvider>());

                context.Database.EnsureCreated();

                Seed(context).GetAwaiter().GetResult();

                return context;
            });

            //Time provider
            services.AddSingleton(services => TimeProviderMother.MondayInBusinessHoursTimeWindowMorning());
            services.AddSingleton(services =>
            {
				//(For application operating rules tests)
				//Whenever a Time-window-rule-violation-test header is present in the request, an access attempt datetime that goes against the
				//time window application operating rule is injected so the application rejects the request, making tests of such scenarios possible
				var httpContext = services.GetRequiredService<IHttpContextAccessor>().HttpContext!;
                
                if (httpContext.Request.Headers.ContainsKey("Time-window-rule-violation-test"))
                    return TimeProviderMother.MondayBeforeBusinessHoursTimeWindow();

				//Whenever a Day-rule-violation-test header is present in the request, an access attempt datetime that goes against the
				//buiness days application operating rules is injected so the application rejects the request, making tests of such scenarios possible
				if (httpContext.Request.Headers.ContainsKey("Day-rule-violation-test"))
					return TimeProviderMother.Saturday();

				return TimeProviderMother.MondayInBusinessHoursTimeWindowMorning();
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
			//services.TryAddScoped<ICacheProvider, RedisCacheProvider>();
			services.TryAddScoped<ICacheProvider, HybridCacheProvider>();

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

            //Based on WireMock Testcontainers package
            services.AddHttpClient();
            services.AddScoped<IViaCepServiceClient>(services =>
            {
				var httpClientFactory = services.GetRequiredService<IHttpClientFactory>();

                var wireMockServerUri = new Uri($"http://127.0.0.1:{_wireMockContainer.GetMappedPublicPort(8080)}");
				var viaCepHttpClient = httpClientFactory.CreateClient("ViaCep");
				viaCepHttpClient.BaseAddress = wireMockServerUri;

				var viaCepIntegrationSettings = Services.GetRequiredService<ViaCepIntegrationSettings>()
                                            with
                                            {
                                                Uri = wireMockServerUri.ToString()
				                            };                

				return new ViaCepServiceClient(
					viaCepHttpClient,
					viaCepIntegrationSettings,
                    default!);

				//https://github.com/WireMock-Net/WireMock.Net/wiki/Using-WireMock.Net.Testcontainers
				//https://www.youtube.com/watch?v=l39pTG31VmA "WireMock With Testcontainers"
				//https://wiremock.org/docs/request-matching/
				//https://antondevtips.com/blog/how-to-test-integrations-with-apis-using-wiremock-in-dotnet (showcases mapping complex matching patterns)

				//How does it work?
				//First, the unit test invokes the LeadManager Api via http client.
				//(The handler that receives the request when the Api is invoked has a dependency on IViaCepServiceClient
				//that is passed via constructor injection.)
				//When the handler class constructor is invoked, this very code is run and all it does is to provide a custom implementation
				//of a http client instance which base Uri points to the ViaCep Mock http server.
			});
            services.AddScoped<IAddressSearch>(sp => sp.GetRequiredService<IViaCepServiceClient>());

			//Based on RichardSzalay.MockHttp package. Works great, by the way.
			//services.AddScoped<IViaCepServiceClient>(services =>
			//{
			//    var leadManagerApiSettings = Services.GetRequiredService<LeadManagerApiSettings>();
			//    var viaCepApiSettings = Services.GetRequiredService<ViaCepIntegrationSettings>();
                  
			//    var httpContextAccessor = services.GetRequiredService<IHttpContextAccessor>();
			//    if (!httpContextAccessor.HttpContext!.Request.Query.TryGetValue("cep", out var cep))
			//        cep = Endereco.CepInvalido;

			//    _httpHandlerMock
			//        .When($"{viaCepApiSettings.Uri}/{string.Format(viaCepApiSettings.Endpoint, cep!)}")
			//        .Respond(
			//            HttpStatusCode.OK,
			//            JsonContent.Create(cep.Equals(Endereco.CepInvalido) ? AddressMother.NotFoundAddress() : AddressMother.FullAddress())
			//        );
                  
			//    var httpClient = _httpHandlerMock.ToHttpClient();
			//    httpClient.BaseAddress = new Uri(viaCepApiSettings.Uri);
			//    httpClient.Timeout = TimeSpan.FromSeconds(viaCepApiSettings.RequestTimeoutInSeconds);
			//    httpClient.DefaultRequestHeaders.Add(
			//        leadManagerApiSettings.ApiKeyRequestHeaderName,
			//        leadManagerApiSettings.ApiKeyRequestHeaderValue);
                  
			//    return new ViaCepServiceClient(httpClient, viaCepApiSettings, default!);
			//});
		});
    }
}