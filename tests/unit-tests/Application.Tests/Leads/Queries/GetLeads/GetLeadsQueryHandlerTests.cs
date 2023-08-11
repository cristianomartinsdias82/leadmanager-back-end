using Application.Contracts.Persistence;
using Application.Features.Leads.Queries.GetLeads;
using Application.Features.Leads.Queries.Shared;
using Application.Tests.Utils.Factories;
using Core.Entities;
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
    private readonly CancellationTokenSource _cts;

    public GetLeadsQueryHandlerTests()
    {
        _cts = new();
        _dbContext = InMemoryLeadManagerDbContextFactory.Create();
        _handler = new(Substitute.For<IMediator>(), _dbContext);
    }

    [Fact]
    public async void Handle_WhenNoLeads_ShouldSuccessfullyReturnEmptyData()
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
    public async void Handle_WhenContainsLeads_ShouldSuccessfullyReturnData()
    {
        //Arrange
        var leads = new List<Lead>()
        {
            LeadMother.XptoLLC(),
            LeadMother.GumperInc(),
        };
        await _dbContext.Leads.AddRangeAsync(leads);
        await _dbContext.SaveChangesAsync(_cts.Token);

        //Act
        var result = await _handler.Handle(new(default!), _cts.Token);

        //Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<ApplicationResponse<IEnumerable<LeadDto>>>();
        result.Success.Should().BeTrue();
        result.Exception.Should().BeNull();
        result.Inconsistencies.Should().BeNullOrEmpty();
        result.Data.Should().NotBeNull().And.HaveCount(leads.Count);
        result.Data.Any(x => x.Id == leads[0].Id).Should().BeTrue();
        result.Data.Any(x => x.Id == leads[1].Id).Should().BeTrue();
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