using Application.Core.Contracts.Repository.Prospecting;
using Application.Prospecting.Leads.Queries.GetLeads;
using Domain.Prospecting.Entities;
using FluentAssertions;
using MediatR;
using NSubstitute;
using Shared.DataPagination;
using Shared.Results;
using Tests.Common.ObjectMothers.Application;
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
	public async Task Handle_Get_SearchTermNotInformed_NoDataInDataSource_ReturnsNoData()
	{
		//Arrange
		var paginationOptions = new PaginationOptions();
		_leadRepositoryMock
			.GetAsync(default, paginationOptions, _cts.Token)
			.Returns(PagedList<Lead>.Empty());

		//Act
		var result = await _handler.Handle(new(default, paginationOptions), _cts.Token);

		//Assert
		result.Should().NotBeNull();
		result.Should().BeOfType<ApplicationResponse<PagedList<LeadDto>>>();
		result.Success.Should().BeTrue();
		result.Exception.Should().BeNull();
		result.Inconsistencies.Should().BeNullOrEmpty();
		result.Data.Should().NotBeNull();
		result.Data.Items.Should().NotBeNull().And.HaveCount(0);
	}

	[Theory]
	//[MemberData(nameof(LeadSearchMatchingTerms))]
	//[MemberData(nameof(LeadSearchNonMatchingTerms))]
	[ClassData(typeof(LeadSearchMatchingTermsExemplars))]
	[ClassData(typeof(LeadSearchNonMatchingTermsExemplars))]
	public async Task Handle_Get_SearchTermInformed_NoDataInDataSource_ReturnsNoData(string search)
	{
		//Arrange
		var paginationOptions = new PaginationOptions();
		_leadRepositoryMock
			.GetAsync(default, paginationOptions, _cts.Token)
			.Returns(PagedList<Lead>.Empty());

		//Act
		var result = await _handler.Handle(new(search, paginationOptions), _cts.Token);

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
	public async Task Handle_Get_SearchTermNotInformed_ReturnsData()
	{
		//Arrange
		var paginationOptions = new PaginationOptions();
		var leads = LeadMother.Leads().ToList();
		_leadRepositoryMock.GetAsync(default, new(), _cts.Token)
						  .Returns(PagedList<Lead>.Paginate(leads, paginationOptions));

		//Act
		var result = await _handler.Handle(new(default, paginationOptions), _cts.Token);

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
		await _leadRepositoryMock.Received(1).GetAsync(default, Arg.Any<PaginationOptions>(), _cts.Token);
	}

	[Theory]
	//[MemberData(nameof(LeadSearchNonMatchingTerms))]
	[ClassData(typeof(LeadSearchNonMatchingTermsExemplars))]
	public async Task Handle_Get_SearchTermInformed_WithNoMatches_ExistingData_ReturnsNoData(string search)
	{
		//Arrange
		var paginationOptions = new PaginationOptions();
		var leads = LeadMother
						.Leads()
						.Where(ld => ld.RazaoSocial.Contains(search) || ld.Cnpj.Contains(search))
						.ToList();
		_leadRepositoryMock
			.GetAsync(search, paginationOptions, _cts.Token)
			.Returns(PagedList<Lead>.Paginate(leads, paginationOptions));

		//Act
		var result = await _handler.Handle(new(search, paginationOptions), _cts.Token);

		//Assert
		result.Should().NotBeNull();
		result.Should().BeOfType<ApplicationResponse<PagedList<LeadDto>>>();
		result.Success.Should().BeTrue();
		result.Exception.Should().BeNull();
		result.Inconsistencies.Should().BeNullOrEmpty();
		result.Data.Should().NotBeNull();
		result.Data.Items.Should().NotBeNull().And.HaveCount(0);
		await _leadRepositoryMock.Received(1).GetAsync(search, Arg.Any<PaginationOptions>(), _cts.Token);
	}

	[Theory]
	[ClassData(typeof(LeadSearchMatchingTermsExemplars))]
	//[MemberData(nameof(LeadSearchMatchingTerms))]
	public async Task Handle_Get_SearchTermInformed_WithMatches_ExistingData_ReturnsData(string search)
	{
		//Arrange
		var paginationOptions = new PaginationOptions();
		var leads = LeadMother
						.Leads()
						.Where(ld => ld.Cnpj.Contains(search) || ld.RazaoSocial.Contains(search)).ToList();
		_leadRepositoryMock.GetAsync(search, new(), _cts.Token)
						  .Returns(PagedList<Lead>.Paginate(leads, paginationOptions));

		//Act
		var result = await _handler.Handle(new(search, paginationOptions), _cts.Token);

		//Assert
		result.Should().NotBeNull();
		result.Should().BeOfType<ApplicationResponse<PagedList<LeadDto>>>();
		result.Success.Should().BeTrue();
		result.Exception.Should().BeNull();
		result.Inconsistencies.Should().BeNullOrEmpty();
		result.Data.Should().NotBeNull();
		result.Data.Items.Should().NotBeNull().And.HaveCountGreaterThanOrEqualTo(1);
		await _leadRepositoryMock.Received(1).GetAsync(search, Arg.Any<PaginationOptions>(), _cts.Token);
	}

	public void Dispose()
	{
		_cts.Dispose();
	}

	//Used in conjunction with MemberData attribute
	//public static IEnumerable<object[]> LeadSearchMatchingTerms()
	//{
	//	yield return new object[] { LeadMother.GumperInc().RazaoSocial };
	//	yield return new object[] { LeadMother.XptoLLC().RazaoSocial };
	//}

	//Used in conjunction with MemberData attribute
	//public static IEnumerable<object[]> LeadSearchNonMatchingTerms()
	//{
	//	yield return new object[] { "this be the good old days" };
	//	yield return new object[] { "le le le le" };
	//	yield return new object[] { "123.46878.24456" };
	//	yield return new object[] { "45654" };
	//}
}