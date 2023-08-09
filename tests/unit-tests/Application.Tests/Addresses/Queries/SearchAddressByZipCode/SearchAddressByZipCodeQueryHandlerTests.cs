using Application.Features.Addresses.Queries.SearchAddressByZipCode;
using FluentAssertions;
using NSubstitute;
using Shared.Results;
using Tests.Common.ObjectMothers.Integrations.ViaCep;
using ViaCep.ServiceClient;
using ViaCep.ServiceClient.Models;
using Xunit;

namespace Application.Tests.Addresses.Queries.SearchAddressByZipCode;

public sealed class SearchAddressByZipCodeQueryHandlerTests : IDisposable
{
    private readonly IViaCepServiceClient _viaCepServiceClient;
    private readonly CancellationTokenSource _cts;

    public SearchAddressByZipCodeQueryHandlerTests()
    {
        _viaCepServiceClient = Substitute.For<IViaCepServiceClient>();
        _cts = new();
    }

    public void Dispose()
    {
        _cts.Dispose();
    }

    [Fact]
    public async Task Handle_AddressFound_ShouldSucceedWithAddressData()
    {
        //Arrange
        _viaCepServiceClient
            .SearchAsync(Arg.Any<string>(), _cts.Token)
            .Returns(AddressMother.FullAddress());
        var request = new SearchAddressByZipCodeQueryRequest(default!);
        var handler = new SearchAddressByZipCodeQueryHandler(_viaCepServiceClient);

        //Act
        var result = await handler.Handle(request, _cts.Token);

        //Assert
        result.Should().NotBeNull();
        result.Message.Should().BeNullOrEmpty();
        result.OperationCode.Should().Be(OperationCodes.Successful);
        await _viaCepServiceClient.Received(1).SearchAsync(Arg.Any<string>(), _cts.Token);
    }

    [Fact]
    public async Task Handle_AddressNotFound_ShouldSucceedWithEmptyAddressData()
    {
        //Arrange
        _viaCepServiceClient
            .SearchAsync(Arg.Any<string>(), _cts.Token)
            .Returns((Endereco)default);
        var request = new SearchAddressByZipCodeQueryRequest(default!);
        var handler = new SearchAddressByZipCodeQueryHandler(_viaCepServiceClient);

        //Act
        var result = await handler.Handle(request, _cts.Token);

        //Assert
        result.Should().NotBeNull();
        result.Message.Should().NotBeNullOrEmpty().And.BeEquivalentTo("Endereço não localizado.");
        result.OperationCode.Should().Be(OperationCodes.Successful);
        await _viaCepServiceClient.Received(1).SearchAsync(Arg.Any<string>(), _cts.Token);
    }

    [Fact]
    public async Task Handle_ErrorOnAddressSearchService_ShouldSucceedWithEmptyAddressData()
    {
        //Arrange
        _viaCepServiceClient
            .SearchAsync(Arg.Any<string>(), _cts.Token)
            .Returns(AddressMother.FaultedAddress());
        var request = new SearchAddressByZipCodeQueryRequest(default!);
        var handler = new SearchAddressByZipCodeQueryHandler(_viaCepServiceClient);

        //Act
        var result = await handler.Handle(request, _cts.Token);

        //Assert
        result.Should().NotBeNull();
        result.Message.Should().NotBeNullOrEmpty().And.BeEquivalentTo("Endereço não localizado.");
        result.OperationCode.Should().Be(OperationCodes.Successful);
        await _viaCepServiceClient.Received(1).SearchAsync(Arg.Any<string>(), _cts.Token);
    }
}
