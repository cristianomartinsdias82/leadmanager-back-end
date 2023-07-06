using Application.Features.Addresses.Queries.SearchAddressByZipCode;
using FluentAssertions;
using Moq;
using Shared.Results;
using Tests.Common.ObjectMothers.Integrations.ViaCep;
using ViaCep.ServiceClient;
using ViaCep.ServiceClient.Models;
using Xunit;

namespace Application.Tests.Addresses.Queries.SearchAddressByZipCode;

public sealed class SearchAddressByZipCodeQueryHandlerTests
{
    private readonly Mock<IViaCepServiceClient> _viaCepServiceClient;

    public SearchAddressByZipCodeQueryHandlerTests()
    {
        _viaCepServiceClient = new Mock<IViaCepServiceClient>();
    }

    [Fact]
    public async Task Handle_AddressFound_ShouldSucceedWithAddressData()
    {
        //Arrange
        using var cts = new CancellationTokenSource();
        _viaCepServiceClient
            .Setup(client => client.SearchAsync(It.IsAny<string>(), cts.Token))
            .ReturnsAsync(AddressMother.FullAddress());
        var request = new SearchAddressByZipCodeQueryRequest(default!);
        var handler = new SearchAddressByZipCodeQueryHandler(_viaCepServiceClient.Object);

        //Act
        var result = await handler.Handle(request, cts.Token);

        //Assert
        result.Should().NotBeNull();
        result.Message.Should().BeNullOrEmpty();
        result.OperationCode.Should().Be(OperationCodes.Successful);
        _viaCepServiceClient.Verify(client => client.SearchAsync(
                                                It.IsAny<string>(),
                                                It.Is<CancellationToken>(tk => tk == cts.Token)), Times.Once());
    }

    [Fact]
    public async Task Handle_AddressNotFound_ShouldSucceedWithEmptyAddressData()
    {
        //Arrange
        using var cts = new CancellationTokenSource();
        _viaCepServiceClient
            .Setup(client => client.SearchAsync(It.IsAny<string>(), cts.Token))
            .ReturnsAsync((Endereco)default);
        var request = new SearchAddressByZipCodeQueryRequest(default!);
        var handler = new SearchAddressByZipCodeQueryHandler(_viaCepServiceClient.Object);

        //Act
        var result = await handler.Handle(request, cts.Token);

        //Assert
        result.Should().NotBeNull();
        result.Message.Should().NotBeNullOrEmpty().And.BeEquivalentTo("Endereço não localizado.");
        result.OperationCode.Should().Be(OperationCodes.Successful);
        _viaCepServiceClient.Verify(client => client.SearchAsync(
                                                It.IsAny<string>(),
                                                It.Is<CancellationToken>(tk => tk == cts.Token)), Times.Once());
    }

    [Fact]
    public async Task Handle_ErrorOnAddressSearchService_ShouldSucceedWithEmptyAddressData()
    {
        //Arrange
        using var cts = new CancellationTokenSource();
        _viaCepServiceClient
            .Setup(client => client.SearchAsync(It.IsAny<string>(), cts.Token))
            .ReturnsAsync(AddressMother.FaultedAddress());
        var request = new SearchAddressByZipCodeQueryRequest(default!);
        var handler = new SearchAddressByZipCodeQueryHandler(_viaCepServiceClient.Object);

        //Act
        var result = await handler.Handle(request, cts.Token);

        //Assert
        result.Should().NotBeNull();
        result.Message.Should().NotBeNullOrEmpty().And.BeEquivalentTo("Endereço não localizado.");
        result.OperationCode.Should().Be(OperationCodes.Successful);
        _viaCepServiceClient.Verify(client => client.SearchAsync(
                                                It.IsAny<string>(),
                                                It.Is<CancellationToken>(tk => tk == cts.Token)), Times.Once());
    }
}
