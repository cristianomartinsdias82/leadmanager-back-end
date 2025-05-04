using Application.Core.Contracts.Repository.Prospecting;
using Application.Prospecting.Leads.Commands.BulkRemoveLeadsFiles;
using CrossCutting.FileStorage;
using CrossCutting.FileStorage.Configuration;
using FluentAssertions;
using MediatR;
using NSubstitute;
using Xunit;

namespace Application.Tests.Prospecting.Leads.Commands.BulkRemoveLeadsFiles;

public sealed class BulkRemoveLeadsFilesCommandRequestHandlerTests
{
	[Fact]
	public async Task Handle_ExistingLeadsFilesIdsInformed_RemovesSuccessfully()
	{
		using var cts = new CancellationTokenSource();
		var token = cts.Token;
		var mediatorMock = Substitute.For<IMediator>();
		var leadsRepositoryMock = Substitute.For<ILeadRepository>();
		var fileStorageProviderMock = Substitute.For<IFileStorageProvider>();
		var storageProviderSettingsMock = Substitute.For<StorageProviderSettings>();
		var sut = new BulkRemoveLeadsFilesCommandRequestHandler(
							mediatorMock,
							leadsRepositoryMock,
							fileStorageProviderMock,
							storageProviderSettingsMock);

		var result = await sut.Handle(new() { Ids = [new RemoveLeadsFileDto(Guid.NewGuid(), "File 1")] }, token);

		await leadsRepositoryMock
				.Received(1)
				.RemoveLeadsFilesByIdsAsync(Arg.Any<IEnumerable<Guid>>(), token);
		
		await fileStorageProviderMock
				.Received(1)
				.BatchRemoveAsync(Arg.Any<IEnumerable<string>>(), Arg.Any<string>(), token);

		result.Success.Should().BeTrue();
		result.OperationCode.Should().Be(Shared.Results.OperationCodes.Successful);
	}
}
