using Application.Contracts.Persistence;
using Application.Features.Leads.Queries.GetLeads;
using Application.Features.Leads.Shared;
using Application.Tests.Utils.Factories;
using Core.Entities;
using CrossCutting.Caching;
using FluentAssertions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Shared.Results;
using Tests.Common.ObjectMothers.Core;
using Xunit;

namespace Application.Tests.Leads.Queries.GetLeads;

public sealed class GetLeadsQueryHandlerTests : IAsyncDisposable, IDisposable
{
    private readonly GetLeadsQueryHandler _handler;
    private readonly ILeadManagerDbContext _dbContext;
    private readonly ICacheProvider _cacheProviderMock;
    private readonly CancellationTokenSource _cts;

    public GetLeadsQueryHandlerTests()
    {
        _cts = new();
        _dbContext = InMemoryLeadManagerDbContextFactory.Create();
        _cacheProviderMock = Substitute.For<ICacheProvider>();
        _handler = new(
            Substitute.For<IMediator>(),
            _cacheProviderMock,
            _dbContext);
    }

    [Fact]
    public async void Handle_NoLeads_ReturnsEmptyData()
    {
        //Arrange && Act
        var result = await _handler.Handle(new(default!), _cts.Token);

        //Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<ApplicationResponse<IEnumerable<LeadDto>>>();
        result.Success.Should().BeTrue();
        result.Exception.Should().BeNull();
        result.Inconsistencies.Should().BeNullOrEmpty();
        result.Data.Should().NotBeNull().And.HaveCount(0);
    }

    [Fact]
    public async void Handle_ExistingLeadsFromDatabase_ReturnsData()
    {
        //Arrange
        var leads = new List<Lead>()
        {
            LeadMother.XptoLLC(),
            LeadMother.GumperInc(),
        };
        await _dbContext.Leads.AddRangeAsync(leads);
        await _dbContext.SaveChangesAsync(_cts.Token);
        _cacheProviderMock.GetAsync<IEnumerable<LeadDto>>(Arg.Any<string>(), _cts.Token)
                          .Returns((IEnumerable<LeadDto>)default!);
        _cacheProviderMock.SetAsync(Arg.Any<string>(), Arg.Any<IEnumerable<LeadDto>>(), cancellationToken: _cts.Token)
                          .Returns(Task.CompletedTask);

        //Act
        var result = await _handler.Handle(new(default!), _cts.Token);

        //Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<ApplicationResponse<IEnumerable<LeadDto>>>();
        result.Success.Should().BeTrue();
        result.Exception.Should().BeNull();
        result.Inconsistencies.Should().BeNullOrEmpty();
        result.Data.Should().NotBeNull().And.HaveCount(leads.Count);
        result.Data.Any(x => x.RazaoSocial == leads[0].RazaoSocial).Should().BeTrue();
        result.Data.Any(x => x.RazaoSocial == leads[1].RazaoSocial).Should().BeTrue();
        await _cacheProviderMock.Received(1).GetAsync<IEnumerable<LeadDto>>(Arg.Any<string>(), _cts.Token);
        await _cacheProviderMock.Received(1).SetAsync<IEnumerable<LeadDto>>(Arg.Any<string>(), Arg.Any<List<LeadDto>>(), cancellationToken: _cts.Token);
    }

    [Fact]
    public async void Handle_ExistingLeadsFromCache_ReturnsData()
    {
        //Arrange
        var leads = new List<Lead>()
        {
            LeadMother.XptoLLC(),
            LeadMother.GumperInc(),
        };
        var leadsDto = leads.ToDtoList();
        _cacheProviderMock.GetAsync<IEnumerable<LeadDto>>(Arg.Any<string>(), _cts.Token)
                          .Returns(leadsDto);

        //Act
        var result = await _handler.Handle(new(default!), _cts.Token);

        //Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<ApplicationResponse<IEnumerable<LeadDto>>>();
        result.Success.Should().BeTrue();
        result.Exception.Should().BeNull();
        result.Inconsistencies.Should().BeNullOrEmpty();
        result.Data.Should().NotBeNull().And.HaveCount(leads.Count);
        result.Data.Any(x => x.RazaoSocial == leads[0].RazaoSocial).Should().BeTrue();
        result.Data.Any(x => x.RazaoSocial == leads[1].RazaoSocial).Should().BeTrue();
        await _cacheProviderMock.Received(1).GetAsync<IEnumerable<LeadDto>>(Arg.Any<string>(), _cts.Token);
        await _cacheProviderMock.Received(0).SetAsync<IEnumerable<LeadDto>>(Arg.Any<string>(), Arg.Any<List<LeadDto>>(), cancellationToken: _cts.Token);
    }

    public async ValueTask DisposeAsync()
    {
        _cts.Dispose();
        await _dbContext.Leads.ExecuteDeleteAsync();
        await _dbContext.DisposeAsync();
    }

    public void Dispose()
    {
        _cts.Dispose();
    }
}