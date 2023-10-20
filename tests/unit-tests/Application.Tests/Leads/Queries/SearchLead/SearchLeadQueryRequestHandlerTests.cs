using Application.Contracts.Persistence;
using Application.Features.Leads.Queries.SearchLead;
using Core.Entities;
using CrossCutting.Security.IAM;
using FluentAssertions;
using MediatR;
using NSubstitute;
using Shared.Results;
using Shared.Tests;
using Tests.Common.ObjectMothers.Core;
using Xunit;

namespace Application.Tests.Leads.Queries.SearchLead;

public sealed class SearchLeadQueryRequestHandlerTests : IAsyncDisposable
{
    private readonly SearchLeadQueryRequestHandler _handler;
    private readonly IUserService _userService;
    private readonly ILeadManagerDbContext _dbContext;
    private readonly IMediator _mediator;
    private static readonly Lead _xptoIncLead = LeadMother.XptoLLC();
    private readonly CancellationTokenSource _cts;
    
    public SearchLeadQueryRequestHandlerTests()
    {
        _mediator = Substitute.For<IMediator>();
        _userService = Substitute.For<IUserService>();
        _userService.GetUserId().Returns(Guid.NewGuid());
        _dbContext = InMemoryLeadManagerDbContextFactory.Create(_userService);
        _handler = new(_mediator, _dbContext);
        _cts = new();
    }

    public async ValueTask DisposeAsync()
    {
        _cts.Dispose();
        await _dbContext.DisposeAsync();
    }

    [Theory]
    [MemberData(nameof(SearchTermsWithMatchesSimulations))]
    public async void Handle_WhenDoesNotContainLeads_ShouldReturnFalse(SearchLeadQueryRequest request)
    {
        //Arrange && Act
        var result = await _handler.Handle(request, _cts.Token);

        //Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<ApplicationResponse<bool>>();
        result.Success.Should().BeTrue();
        result.OperationCode.Should().Be(OperationCodes.Successful);
        result.Data.Should().BeFalse();
    }

    [Theory]
    [MemberData(nameof(SearchTermsWithNoMatchesSimulations))]
    public async void Handle_WhenContainsLeads_And_NoLeadSearchMatches_ShouldReturnFalse(SearchLeadQueryRequest request)
    {
        //Arrange
        await _dbContext.Leads.AddAsync(_xptoIncLead, _cts.Token);
        await _dbContext.SaveChangesAsync(_cts.Token);

        //Act
        var result = await _handler.Handle(request, _cts.Token);

        //Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<ApplicationResponse<bool>>();
        result.Success.Should().BeTrue();
        result.OperationCode.Should().Be(OperationCodes.Successful);
        result.Data.Should().BeFalse();
    }

    [Theory]
    [MemberData(nameof(SearchTermsWithMatchesSimulations))]
    public async void Handle_WhenContainsLeads_And_LeadSearchMatches_ShouldReturnTrue(SearchLeadQueryRequest request)
    {
        //Arrange
        await _dbContext.Leads.AddAsync(_xptoIncLead, _cts.Token);
        await _dbContext.SaveChangesAsync(_cts.Token);

        //Act
        var result = await _handler.Handle(request, _cts.Token);

        //Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<ApplicationResponse<bool>>();
        result.Success.Should().BeTrue();
        result.OperationCode.Should().Be(OperationCodes.Successful);
        result.Data.Should().BeTrue();
    }

    public static IEnumerable<object[]> SearchTermsWithMatchesSimulations()
    {
        yield return new object[] { new SearchLeadQueryRequest(Guid.NewGuid(), _xptoIncLead.Cnpj) };
        //yield return new object[] { new SearchLeadQueryRequest(_xptoIncLead.Id, _xptoIncLead.Cnpj) };
        yield return new object[] { new SearchLeadQueryRequest(Guid.NewGuid(), _xptoIncLead.RazaoSocial) };
        //yield return new object[] { new SearchLeadQueryRequest(_xptoIncLead.Id, _xptoIncLead.RazaoSocial) };
        yield return new object[] { new SearchLeadQueryRequest(default, _xptoIncLead.Cnpj) };
        yield return new object[] { new SearchLeadQueryRequest(default, _xptoIncLead.RazaoSocial) };
    }

    public static IEnumerable<object[]> SearchTermsWithNoMatchesSimulations()
    {
        yield return new object[] { new SearchLeadQueryRequest(Guid.Empty, "32.123.123/0001-23") };
        yield return new object[] { new SearchLeadQueryRequest(Guid.Empty, "Gumper Inc.") };
        yield return new object[] { new SearchLeadQueryRequest(Guid.NewGuid(), "32.123.123/0001-23") };
        yield return new object[] { new SearchLeadQueryRequest(Guid.NewGuid(), "Gumper Inc.") };
        yield return new object[] { new SearchLeadQueryRequest(default, "32.123.123/0001-23") };
        yield return new object[] { new SearchLeadQueryRequest(default, "Gumper Inc.") };
    }
}