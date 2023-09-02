using Application.Contracts.Caching;
using Application.Features.Leads.Queries.GetLeads;
using Application.Features.Leads.Shared;
using FluentAssertions;
using MediatR;
using NSubstitute;
using Shared.DataPagination;
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
        //Arrange
        var paginationOptions = new PaginationOptions();
        _cachingManagerMock
            .GetLeadsAsync(paginationOptions, _cts.Token)
            .Returns(PagedList<LeadDto>.Empty());

        //Act
        var result = await _handler.Handle(new(paginationOptions), _cts.Token);

        //Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<ApplicationResponse<PagedList<LeadDto>>>();
        result.Success.Should().BeTrue();
        result.Exception.Should().BeNull();
        result.Inconsistencies.Should().BeNullOrEmpty();
        result.Data.Should().NotBeNull();
        result.Data.Items.Should().NotBeNull().And.HaveCount(0);
    }

    [Fact]
    public async void Handle_ContainsLeads_ReturnsData()
    {
        //Arrange
        var paginationOptions = new PaginationOptions();
        var leads = LeadMother.Leads();
        _cachingManagerMock.GetLeadsAsync(new(), _cts.Token)
                          .Returns(PagedList<LeadDto>.Paginate(leads.AsDtoList(), paginationOptions));

        //Act
        var result = await _handler.Handle(new(paginationOptions), _cts.Token);

        //Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<ApplicationResponse<PagedList<LeadDto>>>();
        result.Success.Should().BeTrue();
        result.Exception.Should().BeNull();
        result.Inconsistencies.Should().BeNullOrEmpty();
        result.Data.Should().NotBeNull();
        result.Data.Items.Should().NotBeNull().And.HaveCount(leads.Count);
        result.Data.Items.Any(x => x.RazaoSocial == leads[0].RazaoSocial).Should().BeTrue();
        result.Data.Items.Any(x => x.RazaoSocial == leads[1].RazaoSocial).Should().BeTrue();
        await _cachingManagerMock.Received(1).GetLeadsAsync(Arg.Any<PaginationOptions>(), _cts.Token);
    }

    public void Dispose()
    { 
        _cts.Dispose();
    }
}