using Application.Core.Contracts.Repository.Prospecting;
using Application.Prospecting.Leads.Queries.DownloadLeadsFile;
using CrossCutting.FileStorage;
using Domain.Prospecting.Entities;
using FluentAssertions;
using MediatR;
using NSubstitute;
using Shared.Results;
using Tests.Common.ObjectMothers.CrossCutting.FileStorage;
using Tests.Common.ObjectMothers.Domain;
using Xunit;

namespace Application.Tests.Prospecting.Leads.Queries.DownloadLeadsFile;

public sealed class DownloadLeadsFileQueryRequestHandlerTests : IDisposable
{
    private readonly DownloadLeadsFileQueryRequestHandler _sut;
    private readonly IFileStorageProvider _fileStorageProviderMock;
    private readonly IMediator _mediatorMock;
    private readonly ILeadRepository _leadRepositoryMock;
    private readonly CancellationTokenSource _cts = new();

    public DownloadLeadsFileQueryRequestHandlerTests()
    {
        _mediatorMock = Substitute.For<IMediator>();
        _leadRepositoryMock = Substitute.For<ILeadRepository>();
        _fileStorageProviderMock = Substitute.For<IFileStorageProvider>();
        _sut = new(_mediatorMock, _leadRepositoryMock, _fileStorageProviderMock);
    }

	[Fact]
	public async Task Handle_ExistingLeadsFileInformed_ReturnsFile()
	{
		//Arrange
		var leadsFile = LeadsFileMother.File1();
		var fileFromStorage = FileStorageMother.FromLeadsFile(leadsFile);
		var token = _cts.Token;
		DownloadLeadsFileQueryRequest request = new() { Id = Guid.NewGuid() };
		_leadRepositoryMock.GetLeadsFileByIdAsync(Arg.Any<Guid>(), token)
			.Returns(Task.FromResult<LeadsFile?>(leadsFile));
		_fileStorageProviderMock.DownloadAsync(Arg.Any<string>(), cancellationToken: token)
			.Returns(fileFromStorage);

		//Act
		var result = await _sut.Handle(request, _cts.Token);

		//Assert
		result.Should().NotBeNull();
		result.Should().BeOfType<ApplicationResponse<DownloadLeadsFileDto?>>();
		result.Data.Should().NotBeNull();
		result.Data!.FileBytes.SequenceEqual(fileFromStorage.FileBytes).Should().BeTrue(); //https://code-maze.com/dotnet-compare-byte-arrays/
		result.OperationCode.Should().Be(OperationCodes.Successful);
		result.Exception.Should().BeNull();
		await _leadRepositoryMock
				.Received(1)
				.GetLeadsFileByIdAsync(Arg.Any<Guid>(), token);
		await _fileStorageProviderMock
				.Received(1)
				.DownloadAsync(Arg.Any<string>(), cancellationToken: token);
	}

	[Fact]
    public async Task Handle_NonExistingLeadsFileInDbInformed_ReturnsNotFound()
    {
        //Arrange
        var token = _cts.Token;
		DownloadLeadsFileQueryRequest request = new() { Id = Guid.NewGuid() };
        _leadRepositoryMock.GetLeadsFileByIdAsync(Arg.Any<Guid>(), token)
            .Returns(Task.FromResult<LeadsFile?>(null!));

        //Act
        var result = await _sut.Handle(request, _cts.Token);

        //Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<ApplicationResponse<DownloadLeadsFileDto?>>();
        result.Data.Should().BeNull();
        result.OperationCode.Should().Be(OperationCodes.NotFound);
        result.Exception.Should().BeNull();
        await _leadRepositoryMock
                .Received(1)
                .GetLeadsFileByIdAsync(Arg.Any<Guid>(), token);
		await _fileStorageProviderMock
				.DidNotReceive()
				.DownloadAsync(Arg.Any<string>(), cancellationToken: token);
	}

	[Fact]
	public async Task Handle_NonExistingLeadsFileInStorageInformed_ReturnsNotFound()
	{
		//Arrange
		var leadsFile = LeadsFileMother.File1();
		var token = _cts.Token;
		DownloadLeadsFileQueryRequest request = new() { Id = Guid.NewGuid() };
		_leadRepositoryMock.GetLeadsFileByIdAsync(Arg.Any<Guid>(), token)
			.Returns(Task.FromResult<LeadsFile?>(leadsFile));
		_fileStorageProviderMock.DownloadAsync(Arg.Any<string>(), cancellationToken: token)
			.Returns(Task.FromResult<IFile?>(default!));

		//Act
		var result = await _sut.Handle(request, _cts.Token);

		//Assert
		result.Should().NotBeNull();
		result.Should().BeOfType<ApplicationResponse<DownloadLeadsFileDto?>>();
		result.Data.Should().BeNull();
		result.OperationCode.Should().Be(OperationCodes.NotFound);
		result.Exception.Should().BeNull();
		await _leadRepositoryMock
				.Received(1)
				.GetLeadsFileByIdAsync(Arg.Any<Guid>(), token);
		await _fileStorageProviderMock
				.Received(1)
				.DownloadAsync(Arg.Any<string>(), cancellationToken: token);
	}

	public void Dispose()
	{
        _cts.Dispose();
	}
}