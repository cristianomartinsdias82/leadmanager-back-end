using Application.Contracts.Persistence;
using Application.Features.Leads.Queries.SearchLead;
using Application.Tests.Utils.Factories;
using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Tests.Common.ObjectMothers.Core;
using Xunit;
using FluentAssertions;
using Shared.Results;

namespace Application.Tests.Leads.Queries.SearchLead;

public sealed class SearchLeadQueryHandlerTests : IAsyncDisposable
{
    private readonly CancellationTokenSource _cts;
    private readonly ILeadManagerDbContext _dbContext;
    private readonly SearchLeadQueryHandler _handler;
    private static readonly Lead _xptoIncLead = LeadMother.XptoLLC();

    public SearchLeadQueryHandlerTests()
    {
        _cts = new();
        _dbContext = InMemoryLeadManagerDbContextFactory.Create();
        _handler = new(_dbContext);
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

    public async ValueTask DisposeAsync()
    {
        _cts.Dispose();
        await _dbContext.Leads.ExecuteDeleteAsync();
        await _dbContext.DisposeAsync();
    }

    public static IEnumerable<object[]> SearchTermsWithMatchesSimulations()
    {
        yield return new object[] { new SearchLeadQueryRequest(Guid.NewGuid(), _xptoIncLead.Cnpj) };
        yield return new object[] { new SearchLeadQueryRequest(_xptoIncLead.Id, _xptoIncLead.Cnpj) };
        yield return new object[] { new SearchLeadQueryRequest(Guid.NewGuid(), _xptoIncLead.RazaoSocial) };
        yield return new object[] { new SearchLeadQueryRequest(_xptoIncLead.Id, _xptoIncLead.RazaoSocial) };
    }

    public static IEnumerable<object[]> SearchTermsWithNoMatchesSimulations()
    {
        yield return new object[] { new SearchLeadQueryRequest(Guid.Empty, "32.123.123/0001-23") };
        yield return new object[] { new SearchLeadQueryRequest(Guid.Empty, "Gumper Inc.") };
        yield return new object[] { new SearchLeadQueryRequest(Guid.NewGuid(), "32.123.123/0001-23") };
        yield return new object[] { new SearchLeadQueryRequest(Guid.NewGuid(), "Gumper Inc.") };
    }
}