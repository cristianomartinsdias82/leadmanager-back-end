using Application.Contracts.Caching.Policies;
using Application.Contracts.Persistence;
using Application.Features.Leads.Shared;
using CrossCutting.Caching;
using CrossCutting.MessageContracts;
using FluentAssertions;
using Infrastructure.Caching;
using NSubstitute;
using Shared.DataPagination;
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
    private readonly List<LeadDto> _leadDtos;

    public CacheManagerTests()
    {
        _cts = new CancellationTokenSource();
        _dbContext = InMemoryLeadManagerDbContextFactory.Create();
        _cacheProviderMock = Substitute.For<ICacheProvider>();
        _cachingPoliciesSettings = new() { LeadsPolicy = new(default!, default) };
        _cacheManager = new CacheManager(
            _dbContext,
            _cacheProviderMock,
            _cachingPoliciesSettings);
        _leadDtos = LeadMother.Leads().AsDtoList();
    }

    [Fact]
    public async Task AddLeadEntry_InvalidArgument_ThrowsNullArgumentException()
    {
        //Act && Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await _cacheManager.AddLeadEntryAsync(default!, _cts.Token));
        await _cacheProviderMock
                .Received(0)
                .GetAsync<IEnumerable<LeadData>>(
                    key: Arg.Any<string>(),
                    cancellationToken: _cts.Token);
    }

    [Fact]
    public async Task AddLeadEntry_ValidArgument_ShouldRunSuccessfully()
    {
        //Arrange
        var leadDto = LeadMother.XptoLLC().AsDto();
       
        //Act
        await _cacheManager.AddLeadEntryAsync(leadDto, _cts.Token);

        //Assert
        await _cacheProviderMock
                .Received(1)
                .GetAsync<IEnumerable<LeadData>>(
                    key: Arg.Any<string>(),
                    cancellationToken: _cts.Token);
        await _cacheProviderMock
                .Received(1)
                .SetAsync(
                    key: Arg.Any<string>(),
                    item: Arg.Any<IEnumerable<LeadData>>(),
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
                .GetAsync<IEnumerable<LeadData>>(
                    key: Arg.Any<string>(),
                    cancellationToken: _cts.Token);
        await _cacheProviderMock
                .Received(0)
                .SetAsync(
                    key: Arg.Any<string>(),
                    item: Arg.Any<IEnumerable<LeadData>>(),
                    ttlInSeconds: Arg.Any<int>(),
                    cancellationToken: _cts.Token);
    }

    [Fact]
    public async Task AddLeadEntries_ValidArgument_ShouldRunSuccessfully()
    {
        //Arrange && Act
        await _cacheManager.AddLeadEntriesAsync(_leadDtos, _cts.Token);

        //Assert
        await _cacheProviderMock
                .Received(1)
                .GetAsync<IEnumerable<LeadData>>(
                    key: Arg.Any<string>(),
                    cancellationToken: _cts.Token);

        await _cacheProviderMock
                .Received(1)
                .SetAsync(
                    key: Arg.Any<string>(),
                    item: Arg.Any<IEnumerable<LeadData>>(),
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
                .GetAsync<IEnumerable<LeadData>>(
                    key: Arg.Any<string>(),
                    cancellationToken: _cts.Token);

        await _cacheProviderMock
                .Received(0)
                .SetAsync(
                    key: Arg.Any<string>(),
                    item: Arg.Any<IEnumerable<LeadData>>(),
                    ttlInSeconds: Arg.Any<int>(),
                    cancellationToken: _cts.Token);
    }

    [Fact]
    public async Task UpdateLeadEntry_ValidArgument_ExistingLead_ShouldRunSuccessfully()
    {
        //Arrange
        _cacheProviderMock
            .GetAsync<IEnumerable<LeadData>>(Arg.Any<string>(), _cts.Token)
            .Returns(_leadDtos.AsMessageContractList());

        var updatedLead = _leadDtos.First();     

        //Act
        await _cacheManager.UpdateLeadEntryAsync(updatedLead, _cts.Token);

        //Assert
        await _cacheProviderMock
                .Received(1)
                .GetAsync<IEnumerable<LeadData>>(
                    key: Arg.Any<string>(),
                    cancellationToken: _cts.Token);

        await _cacheProviderMock
                .Received(1)
                .SetAsync(
                    key: Arg.Any<string>(),
                    item: Arg.Any<IEnumerable<LeadData>>(),
                    ttlInSeconds: Arg.Any<int>(),
                    cancellationToken: _cts.Token);
    }

    [Fact]
    public async Task UpdateLeadEntry_ValidArgument_NonExistingLead_ShouldRunSuccessfully()
    {
        //Arrange
        _cacheProviderMock
            .GetAsync<IEnumerable<LeadData>>(Arg.Any<string>(), _cts.Token)
            .Returns(Enumerable.Empty<LeadData>());

        //Act
        await _cacheManager.UpdateLeadEntryAsync(LeadMother.GumperInc().AsDto(), _cts.Token);

        //Assert
        await _cacheProviderMock
                .Received(1)
                .GetAsync<IEnumerable<LeadData>>(
                    key: Arg.Any<string>(),
                    cancellationToken: _cts.Token);
        await _cacheProviderMock
                .Received(0)
                .SetAsync(
                    key: Arg.Any<string>(),
                    item: Arg.Any<IEnumerable<LeadData>>(),
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
                .GetAsync<IEnumerable<LeadData>>(
                    key: Arg.Any<string>(),
                    cancellationToken: _cts.Token);
        await _cacheProviderMock
                .Received(0)
                .SetAsync(
                    key: Arg.Any<string>(),
                    item: Arg.Any<IEnumerable<LeadData>>(),
                    ttlInSeconds: Arg.Any<int>(),
                    cancellationToken: _cts.Token);
    }

    [Fact]
    public async Task RemoveLeadEntry_ValidArgument_ExistingLead_ShouldRunSuccessfully()
    {
        //Arrange
        _cacheProviderMock
            .GetAsync<IEnumerable<LeadData>>(Arg.Any<string>(), _cts.Token)
            .Returns(_leadDtos.AsMessageContractList());

        var leadToRemove = _leadDtos.First();

        //Act
        await _cacheManager.RemoveLeadEntryAsync(leadToRemove, _cts.Token);

        //Assert
        await _cacheProviderMock
                .Received(1)
                .GetAsync<IEnumerable<LeadData>>(
                    key: Arg.Any<string>(),
                    cancellationToken: _cts.Token);
        await _cacheProviderMock
                .Received(1)
                .SetAsync(
                    key: Arg.Any<string>(),
                    item: Arg.Any<IEnumerable<LeadData>>(),
                    ttlInSeconds: Arg.Any<int>(),
                    cancellationToken: _cts.Token);
    }

    [Fact]
    public async Task RemoveLeadEntry_ValidArgument_NonExistingLead_ShouldRunSuccessfully()
    {
        //Arrange
        _cacheProviderMock
            .GetAsync<IEnumerable<LeadData>>(Arg.Any<string>(), _cts.Token)
            .Returns(Enumerable.Empty<LeadData>());

        //Act
        await _cacheManager.RemoveLeadEntryAsync(LeadMother.XptoLLC().AsDto(), _cts.Token);

        //Assert
        await _cacheProviderMock
                .Received(1)
                .GetAsync<IEnumerable<LeadData>>(
                    key: Arg.Any<string>(),
                    cancellationToken: _cts.Token);

        await _cacheProviderMock
                .Received(0)
                .SetAsync(
                    key: Arg.Any<string>(),
                    item: Arg.Any<IEnumerable<LeadData>>(),
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
    public async Task GetLeads_ListWithData_ReturnsLeadsList()
    {
        //Arrange
        _cacheProviderMock.GetAsync<IEnumerable<LeadData>>(Arg.Any<string>(), _cts.Token)
                          .Returns(_leadDtos.AsMessageContractList());
        
        //Act
        var cachedLeads = await _cacheManager.GetLeadsAsync(new(), _cts.Token);

        //Assert
        await _cacheProviderMock
                .Received(1)
                .GetAsync<IEnumerable<LeadData>>(Arg.Any<string>(), _cts.Token);
        _leadDtos.Should().NotBeNull().And.HaveCount(cachedLeads.ItemCount);
        for(var i = 0; i < _leadDtos.Count; i++)
            _leadDtos[i].Id.Should().Be(cachedLeads.Items.ElementAt(i).Id);
    }

    [Fact]
    public async Task GetLeads_EmptyList_ReturnsEmptyList()
    {
        //Arrange & act
        var cachedLeads = await _cacheManager.GetLeadsAsync(new(), _cts.Token);

        //Assert
        await _cacheProviderMock
                .Received(1)
                .GetAsync<IEnumerable<LeadData>>(Arg.Any<string>(), _cts.Token);
        cachedLeads.ItemCount.Should().Be(0);
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
