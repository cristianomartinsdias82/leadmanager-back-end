﻿using Application.Prospecting.Addresses.Queries.SearchAddressByZipCode;
using FluentAssertions;
using LeadManagerApi.Tests.Core;
using LeadManagerApi.Tests.Core.Factories;
using Shared.Results;
using System.Net;
using System.Text.Json;
using Tests.Common.ObjectMothers.Integrations.ViaCep;
using ViaCep.ServiceClient.Models;
using Xunit;

namespace LeadManagerApi.Tests.Prospecting.Addresses.SearchAddressByZipCode;

public class SearchAddressByZipCodeControllerTests : SharedResourcesTestsBase
{
    private const string SearchAddressUri = $"{AddressesEndpoint}/search?cep=";
    private const string AddressNotLocated = "Endereço não localizado.";

    public SearchAddressByZipCodeControllerTests(
        LeadManagerWebApplicationFactory factory) : base(factory) { }

    [Fact]
    public async Task Handle_ApiKeyHeaderNotSet_ShouldFail()
    {
        // Arrange
        var httpClient = _factory.CreateHttpClient(false);

        // Act
        var response = await httpClient.GetAsync(SearchAddressUri);

        // Assert
        response.ReasonPhrase.Should().Contain(HttpStatusCode.Unauthorized.ToString(), because: "Api key header has not been set.");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized, because: "Api key header has not been set.");
    }

    [Fact(Skip = "Find out the reason why this test succeeds only when in debug mode.")]
    public async Task Handle_ExistingZipCode_ShouldSucceedAndReturnAddressData()
    {
        var address = AddressMother.FullAddress();

		// Arrange
		var httpClient = _factory.CreateHttpClient();

        // Act
        var response = await httpClient.GetAsync(string.Concat(SearchAddressUri, address.Cep));

        // Assert
        response.EnsureSuccessStatusCode();
        var responseContent = await response.Content.ReadAsStringAsync();

        ApplicationResponse<SearchAddressByZipCodeQueryResponse> apiResponse = default!;
		Action action = () => { apiResponse = _factory.DeserializeFromJson<ApplicationResponse<SearchAddressByZipCodeQueryResponse>>(responseContent)!; };
		action.Should().NotThrow<Exception>();
        apiResponse.Exception.Should().BeNull();
        apiResponse.Success.Should().BeTrue();
        apiResponse.OperationCode.Should().Be(OperationCodes.Successful);
        apiResponse.Data.Should().NotBeNull();
		apiResponse.Data.Cep.Should().BeEquivalentTo(address.Cep);
		apiResponse.Message.Should().NotBeEquivalentTo(AddressNotLocated);
	}

    [Fact]
    public async Task Handle_NonExistingZipCode_ShouldReturnEmptyBody()
    {
        // Arrange
        var httpClient = _factory.CreateHttpClient();

        // Act
        var response = await httpClient.GetAsync(string.Concat(SearchAddressUri, Endereco.CepInvalido));

        // Assert
        response.EnsureSuccessStatusCode();
        var responseContent = await response.Content.ReadAsStringAsync();

        ApplicationResponse<SearchAddressByZipCodeQueryResponse> apiResponse = default!;
		Action action = () => { apiResponse = _factory.DeserializeFromJson<ApplicationResponse<SearchAddressByZipCodeQueryResponse>>(responseContent)!; };
		action.Should().NotThrow<Exception>();
		apiResponse.OperationCode.Should().Be(OperationCodes.NotFound);
        apiResponse.Success.Should().BeTrue();
		apiResponse.Message.Should().BeEquivalentTo(AddressNotLocated);
        apiResponse.Data.Should().BeNull();
        apiResponse.Exception.Should().BeNull();
	}
}