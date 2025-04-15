using Application.AddressSearch.Contracts;
using Application.AddressSearch.Models;
using Application.Prospecting.Addresses.Queries.SearchAddressByZipCode;
using FluentAssertions;
using MediatR;
using NSubstitute;
using Shared.Results;
using Tests.Common.ObjectMothers.Integrations.ViaCep;
//using ViaCep.ServiceClient;
//using ViaCep.ServiceClient.Models;
using Xunit;

namespace Application.Tests.Prospecting.Addresses.Queries.SearchAddressByZipCode;

public sealed class SearchAddressByZipCodeQueryHandlerTests : IDisposable
{
    //private readonly IViaCepServiceClient _viaCepServiceClient;
    private readonly IAddressSearch _addressSearch;
    private readonly CancellationTokenSource _cts;

    public SearchAddressByZipCodeQueryHandlerTests()
    {
        //_viaCepServiceClient = Substitute.For<IViaCepServiceClient>();
		_addressSearch = Substitute.For<IAddressSearch>();
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
		//_viaCepServiceClient
		_addressSearch
			.SearchByZipCodeAsync(Arg.Any<string>(), _cts.Token)
            .Returns(AddressMother.FullAddress());
        var request = new SearchAddressByZipCodeQueryRequest(default!);
        var handler = new SearchAddressByZipCodeQueryHandler(
                            Substitute.For<IMediator>(),
							//_viaCepServiceClient);
							_addressSearch);

        //Act
        var result = await handler.Handle(request, _cts.Token);

        //Assert
        result.Should().NotBeNull();
        result.Message.Should().BeNullOrEmpty();
        result.OperationCode.Should().Be(OperationCodes.Successful);
        //await _viaCepServiceClient.Received(1).SearchByZipCodeAsync(Arg.Any<string>(), _cts.Token);
        await _addressSearch.Received(1).SearchByZipCodeAsync(Arg.Any<string>(), _cts.Token);
    }

    [Fact]
    public async Task Handle_AddressNotFound_ShouldSucceedWithEmptyAddressData()
    {
		//Arrange
		//_viaCepServiceClient
		_addressSearch
			.SearchByZipCodeAsync(Arg.Any<string>(), _cts.Token)
            .Returns((Address)default!);
        var request = new SearchAddressByZipCodeQueryRequest(default!);
        var handler = new SearchAddressByZipCodeQueryHandler(
                            Substitute.For<IMediator>(),
							//_viaCepServiceClient);
							_addressSearch);

        //Act
        var result = await handler.Handle(request, _cts.Token);

        //Assert
        result.Should().NotBeNull();
        result.Message.Should().NotBeNullOrEmpty().And.BeEquivalentTo("Endereço não localizado.");
        result.OperationCode.Should().Be(OperationCodes.NotFound);
        //await _viaCepServiceClient.Received(1).SearchByZipCodeAsync(Arg.Any<string>(), _cts.Token);
        await _addressSearch.Received(1).SearchByZipCodeAsync(Arg.Any<string>(), _cts.Token);
    }

    [Fact]
    public async Task Handle_ErrorOnAddressSearchService_ShouldSucceedWithEmptyAddressData()
    {
		//Arrange
		//_viaCepServiceClient
		_addressSearch
			.SearchByZipCodeAsync(Arg.Any<string>(), _cts.Token)
            .Returns((Address)default!);
        var request = new SearchAddressByZipCodeQueryRequest(default!);
        var handler = new SearchAddressByZipCodeQueryHandler(
                            Substitute.For<IMediator>(),
							//_viaCepServiceClient);
							_addressSearch);

        //Act
        var result = await handler.Handle(request, _cts.Token);

        //Assert
        result.Should().NotBeNull();
        result.Message.Should().NotBeNullOrEmpty().And.BeEquivalentTo("Endereço não localizado.");
        result.OperationCode.Should().Be(OperationCodes.NotFound);
        //await _viaCepServiceClient.Received(1).SearchByZipCodeAsync(Arg.Any<string>(), _cts.Token);
        await _addressSearch.Received(1).SearchByZipCodeAsync(Arg.Any<string>(), _cts.Token);
    }
}
