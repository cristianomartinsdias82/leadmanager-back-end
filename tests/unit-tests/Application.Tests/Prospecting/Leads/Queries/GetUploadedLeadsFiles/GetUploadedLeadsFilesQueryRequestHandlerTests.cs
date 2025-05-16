using Application.Core.Contracts.Repository.Prospecting;
using Application.Prospecting.Leads.Queries.GetUploadedLeadsFiles;
using Domain.Prospecting.Entities;
using FluentAssertions;
using MediatR;
using NSubstitute;
using Shared.DataQuerying;
using Shared.Results;
using Tests.Common.ObjectMothers.Domain;
using Xunit;

namespace Application.Tests.Prospecting.Leads.Queries.GetUploadedLeadsFiles;

public sealed class GetUploadedLeadsFilesQueryRequestHandlerTests : IDisposable
{
	private readonly GetUploadedLeadsFilesQueryRequestHandler _handler;
	private readonly ILeadRepository _leadRepositoryMock;
	private readonly IMediator _mediator;
	private readonly CancellationTokenSource _cts;

	public GetUploadedLeadsFilesQueryRequestHandlerTests()
	{
		_mediator = Substitute.For<IMediator>();
		_leadRepositoryMock = Substitute.For<ILeadRepository>();
		_handler = new(_mediator, _leadRepositoryMock);
		_cts = new();
	}

	[Fact]
	public async Task Handle_Get_NoDataInDataSource_ReturnsNoData()
	{
		//Arrange
		var paginationOptions = PaginationOptions.SinglePage();
		_leadRepositoryMock
			.GetLeadsFilesAsync(paginationOptions, _cts.Token)
			.Returns(PagedList<LeadsFile>.Empty());

		//Act
		var result = await _handler.Handle(new(paginationOptions), _cts.Token);

		//Assert
		result.Should().NotBeNull();
		result.Should().BeOfType<ApplicationResponse<PagedList<UploadedLeadsFileDto>>>();
		result.Success.Should().BeTrue();
		result.Exception.Should().BeNull();
		result.Inconsistencies.Should().BeNullOrEmpty();
		result.Data.Should().NotBeNull();
		result.Data.Items.Should().NotBeNull().And.HaveCount(0);
	}

	[Fact]
	public async Task Handle_Get_WithDataInDataSource_ReturnsData()
	{
		//Arrange
		var paginationOptions = PaginationOptions.SinglePage();
		_leadRepositoryMock
			.GetLeadsFilesAsync(Arg.Any<PaginationOptions>(), _cts.Token)
			.Returns(PagedList<LeadsFile>.Paginate(
				LeadsFileMother.LeadsFiles(),
				paginationOptions));

		//Act
		var result = await _handler.Handle(new(paginationOptions), _cts.Token);

		//Assert
		result.Should().NotBeNull();
		result.Should().BeOfType<ApplicationResponse<PagedList<UploadedLeadsFileDto>>>();
		result.Success.Should().BeTrue();
		result.Exception.Should().BeNull();
		result.Inconsistencies.Should().BeNullOrEmpty();
		result.Data.Should().NotBeNull();
		result.Data.Items.Should().NotBeNull().And.HaveCountGreaterThan(0);
		await _leadRepositoryMock.Received(1)
								 .GetLeadsFilesAsync(
									paginationOptions,
									_cts.Token);
	}

	public void Dispose()
	{
		_cts.Dispose();
	}
}