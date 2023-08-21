using Application.Contracts.Caching.Policies;
using Application.Contracts.Persistence;
using Application.Features.Leads.Shared;
using CrossCutting.Caching;
using FluentAssertions;
using Infrastructure.Caching;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Shared.Tests;
using Tests.Common.ObjectMothers.Core;
using Xunit;

namespace Infrastructure.Tests.Caching;

public sealed class CacheManagerTests : IDisposable, IAsyncDisposable
{
    private readonly CacheManager _cacheManager;
    private readonly ILeadManagerDbContext _dbContext;
    private readonly CancellationTokenSource _cts;
    private readonly ICacheProvider _cacheProviderMock;
    private readonly CachingPoliciesSettings _cachingPoliciesSettings;
    private readonly ILogger<CacheManager> _logger;

    public CacheManagerTests()
    {
        _cts = new CancellationTokenSource();
        _dbContext = InMemoryLeadManagerDbContextFactory.Create();
        _cacheProviderMock = Substitute.For<ICacheProvider>();
        _cachingPoliciesSettings = new() { LeadsPolicy = new(default!, default) };
        _logger = Substitute.For<ILogger<CacheManager>>();
        _cacheManager = new CacheManager(
            _dbContext,
            _cacheProviderMock,
            _cachingPoliciesSettings,
            _logger);        
    }

    [Fact]
    public async Task AddLeadEntry_InvalidArgument_ThrowsNullArgumentException()
    {
        //Act && Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await _cacheManager.AddLeadEntryAsync(default!, _cts.Token));
        await _cacheProviderMock
                .Received(0)
                .GetAsync<IEnumerable<LeadDto>>(
                    key: Arg.Any<string>(),
                    cancellationToken: _cts.Token);
    }

    [Fact]
    public async Task AddLeadEntry_ValidArgument_ShouldRunSuccessfully()
    {
        //Arrange
        var leadDto = LeadMother.XptoLLC().ToDto();
       
        //Act
        await _cacheManager.AddLeadEntryAsync(leadDto, _cts.Token);

        //Assert
        await _cacheProviderMock
                .Received(1)
                .GetAsync<IEnumerable<LeadDto>>(
                    key: Arg.Any<string>(),
                    cancellationToken: _cts.Token);
        await _cacheProviderMock
                .Received(1)
                .SetAsync(
                    key: Arg.Any<string>(),
                    item: Arg.Any<IEnumerable<LeadDto>>(),
                    ttlInSeconds: Arg.Any<int>(),
                    cancellationToken: _cts.Token);
    }

    [Fact]
    public async Task AddLeadEntries_InvalidArgument_ThrowsNullArgumentException()
    {
        //Act && Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await _cacheManager.AddLeadEntriesAsync(default!, _cts.Token));
        await _cacheProviderMock
                .Received(0)
                .GetAsync<IEnumerable<LeadDto>>(
                    key: Arg.Any<string>(),
                    cancellationToken: _cts.Token);
        await _cacheProviderMock
                .Received(0)
                .SetAsync(
                    key: Arg.Any<string>(),
                    item: Arg.Any<IEnumerable<LeadDto>>(),
                    ttlInSeconds: Arg.Any<int>(),
                    cancellationToken: _cts.Token);
    }

    [Fact]
    public async Task AddLeadEntries_ValidArgument_ShouldRunSuccessfully()
    {
        //Arrange
        var leadsDto = LeadMother.Leads().Select(ld => ld.ToDto()).ToList();

        //Act
        await _cacheManager.AddLeadEntriesAsync(leadsDto, _cts.Token);

        //Assert
        await _cacheProviderMock
                .Received(1)
                .GetAsync<IEnumerable<LeadDto>>(
                    key: Arg.Any<string>(),
                    cancellationToken: _cts.Token);
        await _cacheProviderMock
                .Received(1)
                .SetAsync(
                    key: Arg.Any<string>(),
                    item: Arg.Any<IEnumerable<LeadDto>>(),
                    ttlInSeconds: Arg.Any<int>(),
                    cancellationToken: _cts.Token);
    }

    [Fact]
    public async Task UpdateLeadEntry_InvalidArgument_ThrowsNullArgumentException()
    {
        //Act && Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await _cacheManager.UpdateLeadEntryAsync(default!, _cts.Token));
        await _cacheProviderMock
                .Received(0)
                .GetAsync<IEnumerable<LeadDto>>(
                    key: Arg.Any<string>(),
                    cancellationToken: _cts.Token);
        await _cacheProviderMock
                .Received(0)
                .SetAsync(
                    key: Arg.Any<string>(),
                    item: Arg.Any<IEnumerable<LeadDto>>(),
                    ttlInSeconds: Arg.Any<int>(),
                    cancellationToken: _cts.Token);
    }

    [Fact]
    public async Task UpdateLeadEntry_ValidArgument_ExistingLead_ShouldRunSuccessfully()
    {
        //Arrange
        var leadsDto = LeadMother.Leads().Select(ld => ld.ToDto()).ToList();
        _cacheProviderMock
            .GetAsync<IEnumerable<LeadDto>>(Arg.Any<string>(), _cts.Token)
            .Returns(leadsDto);

        var updatedLead = leadsDto.First();     

        //Act
        await _cacheManager.UpdateLeadEntryAsync(updatedLead, _cts.Token);

        //Assert
        await _cacheProviderMock
                .Received(1)
                .GetAsync<IEnumerable<LeadDto>>(
                    key: Arg.Any<string>(),
                    cancellationToken: _cts.Token);
        await _cacheProviderMock
                .Received(1)
                .SetAsync(
                    key: Arg.Any<string>(),
                    item: Arg.Any<IEnumerable<LeadDto>>(),
                    ttlInSeconds: Arg.Any<int>(),
                    cancellationToken: _cts.Token);
    }

    [Fact]
    public async Task UpdateLeadEntry_ValidArgument_NonExistingLead_ShouldRunSuccessfully()
    {
        //Arrange
        var leadsDto = LeadMother.Leads().Select(ld => ld.ToDto()).ToList();
        _cacheProviderMock
            .GetAsync<IEnumerable<LeadDto>>(Arg.Any<string>(), _cts.Token)
            .Returns(Enumerable.Empty<LeadDto>());

        //Act
        await _cacheManager.UpdateLeadEntryAsync(LeadMother.GumperInc().ToDto(), _cts.Token);

        //Assert
        await _cacheProviderMock
                .Received(1)
                .GetAsync<IEnumerable<LeadDto>>(
                    key: Arg.Any<string>(),
                    cancellationToken: _cts.Token);
        await _cacheProviderMock
                .Received(0)
                .SetAsync(
                    key: Arg.Any<string>(),
                    item: Arg.Any<IEnumerable<LeadDto>>(),
                    ttlInSeconds: Arg.Any<int>(),
                    cancellationToken: _cts.Token);
    }

    [Fact]
    public async Task RemoveLeadEntry_InvalidArgument_ThrowsNullArgumentException()
    {
        //Act && Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await _cacheManager.RemoveLeadEntryAsync(default!, _cts.Token));
        await _cacheProviderMock
                .Received(0)
                .GetAsync<IEnumerable<LeadDto>>(
                    key: Arg.Any<string>(),
                    cancellationToken: _cts.Token);
        await _cacheProviderMock
                .Received(0)
                .SetAsync(
                    key: Arg.Any<string>(),
                    item: Arg.Any<IEnumerable<LeadDto>>(),
                    ttlInSeconds: Arg.Any<int>(),
                    cancellationToken: _cts.Token);
    }

    [Fact]
    public async Task RemoveLeadEntry_ValidArgument_ExistingLead_ShouldRunSuccessfully()
    {
        //Arrange
        var leadsDto = LeadMother.Leads().Select(ld => ld.ToDto()).ToList();
        _cacheProviderMock
            .GetAsync<IEnumerable<LeadDto>>(Arg.Any<string>(), _cts.Token)
            .Returns(leadsDto);

        var leadToRemove = leadsDto.First();

        //Act
        await _cacheManager.RemoveLeadEntryAsync(leadToRemove, _cts.Token);

        //Assert
        await _cacheProviderMock
                .Received(1)
                .GetAsync<IEnumerable<LeadDto>>(
                    key: Arg.Any<string>(),
                    cancellationToken: _cts.Token);
        await _cacheProviderMock
                .Received(1)
                .SetAsync(
                    key: Arg.Any<string>(),
                    item: Arg.Any<IEnumerable<LeadDto>>(),
                    ttlInSeconds: Arg.Any<int>(),
                    cancellationToken: _cts.Token);
    }

    [Fact]
    public async Task RemoveLeadEntry_ValidArgument_NonExistingLead_ShouldRunSuccessfully()
    {
        //Arrange
        _cacheProviderMock
            .GetAsync<IEnumerable<LeadDto>>(Arg.Any<string>(), _cts.Token)
            .Returns(Enumerable.Empty<LeadDto>());

        //Act
        await _cacheManager.RemoveLeadEntryAsync(LeadMother.XptoLLC().ToDto(), _cts.Token);

        //Assert
        await _cacheProviderMock
                .Received(1)
                .GetAsync<IEnumerable<LeadDto>>(
                    key: Arg.Any<string>(),
                    cancellationToken: _cts.Token);
        await _cacheProviderMock
                .Received(0)
                .SetAsync(
                    key: Arg.Any<string>(),
                    item: Arg.Any<IEnumerable<LeadDto>>(),
                    ttlInSeconds: Arg.Any<int>(),
                    cancellationToken: _cts.Token);
    }

    [Fact]
    public async Task ClearLeadEntries_RunsSuccessfully()
    {
        //Arrange & act
        await _cacheManager.ClearLeadEntriesAsync(_cts.Token);

        //Assert
        await _cacheProviderMock
                .Received(1)
                .RemoveAsync(Arg.Any<string>(), _cts.Token);
    }

    [Fact]
    public async Task GetLeads_NonEmptyList_ReturnsLeadsList()
    {
        //Arrange
        var leads = LeadMother.Leads().Select(ld => ld.ToDto()).ToList();
        _cacheProviderMock.GetAsync<IEnumerable<LeadDto>>(Arg.Any<string>(), _cts.Token)
                          .Returns(leads);
        
        //Act
        var cachedLeads = await _cacheManager.GetLeadsAsync(_cts.Token);

        //Assert
        await _cacheProviderMock
                .Received(1)
                .GetAsync<IEnumerable<LeadDto>>(Arg.Any<string>(), _cts.Token);
        leads.Count.Should().Be(cachedLeads.Count());
        for(var i = 0; i < leads.Count; i++)
            leads[i].Id = cachedLeads.ElementAt(i).Id;
    }

    [Fact]
    public async Task GetLeads_EmptyList_ReturnsEmptyList()
    {
        //Arrange & act
        var cachedLeads = await _cacheManager.GetLeadsAsync(_cts.Token);

        //Assert
        await _cacheProviderMock
                .Received(1)
                .GetAsync<IEnumerable<LeadDto>>(Arg.Any<string>(), _cts.Token);
        cachedLeads.Count().Should().Be(0);
    }

    public void Dispose()
    {
        _cts.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        await _dbContext.DisposeAsync();
    }
}
