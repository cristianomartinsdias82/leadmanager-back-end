using Application.Core.Contracts.Repository;
using Application.Prospecting.Leads.Queries.SearchLead;
using CrossCutting.Security.IAM;
using Domain.Prospecting.Entities;
using FluentAssertions;
using MediatR;
using NSubstitute;
using Shared.Results;
using System.Linq.Expressions;
using Tests.Common.ObjectMothers.Domain;
using Xunit;

namespace Application.Tests.Prospecting.Leads.Queries.SearchLead;

public sealed class SearchLeadQueryRequestHandlerTests
{
    private readonly SearchLeadQueryRequestHandler _handler;
    private readonly IUserService _userService;
    private readonly ILeadRepository _leadRepository;
    private readonly IMediator _mediator;
    private readonly CancellationTokenSource _cts;
    private static readonly Lead _xptoIncLead = LeadMother.XptoLLC();

    public SearchLeadQueryRequestHandlerTests()
    {
        _mediator = Substitute.For<IMediator>();
        _userService = Substitute.For<IUserService>();
        _userService.GetUserId().Returns(Guid.NewGuid());
        _leadRepository = Substitute.For<ILeadRepository>();
        _handler = new(_mediator, _leadRepository);
        _cts = new();
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
        _leadRepository
            .ExistsAsync(x => true, _cts.Token)
            .Returns(false);

        //Act
        var result = await _handler.Handle(request, _cts.Token);

        //Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<ApplicationResponse<bool>>();
        result.Success.Should().BeTrue();
        result.OperationCode.Should().Be(OperationCodes.Successful);
        result.Data.Should().BeFalse();
    }

    //[Theory]
    //[MemberData(nameof(SearchTermsWithMatchesSimulations))]
    [Fact]
    public async void Handle_WhenContainsLeads_And_LeadSearchMatches_ShouldReturnTrue()//(SearchLeadQueryRequest request)
    {
        //Arrange
        _leadRepository
            .ExistsAsync(Arg.Any<Expression<Func<Lead, bool>>>(), _cts.Token)
            .Returns(true);

        //Act
        //var result = await _handler.Handle(request, _cts.Token);
        var result = await _handler.Handle(new(Guid.NewGuid(), "*"), _cts.Token);

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