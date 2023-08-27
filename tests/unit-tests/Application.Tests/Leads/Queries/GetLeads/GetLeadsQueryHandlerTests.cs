using Application.Contracts.Caching;
using Application.Features.Leads.Queries.GetLeads;
using Application.Features.Leads.Shared;
using CrossCutting.MessageContracts;
using FluentAssertions;
using MediatR;
using NSubstitute;
using Shared.Results;
using Tests.Common.ObjectMothers.Core;
using Xunit;

namespace Application.Tests.Leads.Queries.GetLeads;

public sealed class GetLeadsQueryHandlerTests : IDisposable
{
    private readonly GetLeadsQueryHandler _handler;
    private readonly ICachingManagement _cachingManagerMock;
    private readonly IMediator _mediator;
    private readonly CancellationTokenSource _cts;

    public GetLeadsQueryHandlerTests()
    {
        _mediator = Substitute.For<IMediator>();
        _cachingManagerMock = Substitute.For<ICachingManagement>();
        _handler = new(_mediator, _cachingManagerMock);
        _cts = new();
    }

    [Fact]
    public async void Handle_DoesNotContainLeads_ReturnsEmptyData()
    {
        //Arrange && Act
        var result = await _handler.Handle(new(default!), _cts.Token);
        _cachingManagerMock.GetLeadsAsync(_cts.Token)
                          .Returns(Enumerable.Empty<LeadData>());

        //Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<ApplicationResponse<IEnumerable<LeadDto>>>();
        result.Success.Should().BeTrue();
        result.Exception.Should().BeNull();
        result.Inconsistencies.Should().BeNullOrEmpty();
        result.Data.Should().NotBeNull().And.HaveCount(0);
    }

    [Fact]
    public async void Handle_ContainsLeads_ReturnsData()
    {
        //Arrange
        var leads = LeadMother.Leads();
        _cachingManagerMock.GetLeadsAsync(_cts.Token)
                          .Returns(leads.AsMessageContractList());

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
        await _cachingManagerMock.Received(1).GetLeadsAsync(_cts.Token);
    }

    public void Dispose()
    { 
        _cts.Dispose();
    }
}