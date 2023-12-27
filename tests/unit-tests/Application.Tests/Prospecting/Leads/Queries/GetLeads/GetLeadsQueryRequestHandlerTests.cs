using Application.Core.Contracts.Repository.Prospecting;
using Application.Prospecting.Leads.Queries.GetLeads;
using Domain.Prospecting.Entities;
using FluentAssertions;
using MediatR;
using NSubstitute;
using Shared.DataPagination;
using Shared.Results;
using Tests.Common.ObjectMothers.Domain;
using Xunit;

namespace Application.Tests.Prospecting.Leads.Queries.GetLeads;

public sealed class GetLeadsQueryRequestHandlerTests : IDisposable
{
    private readonly GetLeadsQueryRequestHandler _handler;
    private readonly ILeadRepository _leadRepositoryMock;
    private readonly IMediator _mediator;
    private readonly CancellationTokenSource _cts;

    public GetLeadsQueryRequestHandlerTests()
    {
        _mediator = Substitute.For<IMediator>();
        _leadRepositoryMock = Substitute.For<ILeadRepository>();
        _handler = new(_mediator, _leadRepositoryMock);
        _cts = new();
    }

    [Fact]
    public async void Handle_DoesNotContainLeads_ReturnsEmptyData()
    {
        //Arrange
        var paginationOptions = new PaginationOptions();
        _leadRepositoryMock
            .GetAsync(paginationOptions, _cts.Token)
            .Returns(PagedList<Lead>.Empty());

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
        _leadRepositoryMock.GetAsync(new(), _cts.Token)
                          .Returns(PagedList<Lead>.Paginate(leads, paginationOptions));

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
        await _leadRepositoryMock.Received(1).GetAsync(Arg.Any<PaginationOptions>(), _cts.Token);
    }

    public void Dispose()
    {
        _cts.Dispose();
    }
}