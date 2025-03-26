using Application.Prospecting.Addresses.Queries.SearchAddressByZipCode;
using FluentAssertions;
using LeadManagerApi.Tests.Core;
using LeadManagerApi.Tests.Core.Factories;
using Shared.Results;
using System.Net;
using Tests.Common.ObjectMothers.Integrations.ViaCep;
using ViaCep.ServiceClient.Models;
using Xunit;

namespace LeadManagerApi.Tests.Prospecting.Addresses.SearchAddressByZipCode;

public class SearchAddressByZipCodeControllerTests : SharedResourcesTestsBase
{
    private const string SearchAddressUri = $"{AddressesEndpoint}/search?cep=";

    public SearchAddressByZipCodeControllerTests(
        LeadManagerWebApplicationFactory factory) : base(factory) { }

    [Fact]
    public async Task Get_ApiKeyHeaderNotSet_ShouldFail()
    {
        // Arrange
        var httpClient = _factory.CreateHttpClient(false);

        // Act
        var response = await httpClient.GetAsync(SearchAddressUri);

        // Assert
        response.ReasonPhrase.Should().Contain(HttpStatusCode.Unauthorized.ToString(), because: "Api key header has not been set.");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized, because: "Api key header has not been set.");
    }

    [Fact]
    public async Task Get_ExistingZipCode_ShouldSucceedAndReturnFoundAddress()
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
		apiResponse.Message.Should().NotBeEquivalentTo("Endereço não localizado.");
	}

    [Fact]
    public async Task Get_NonExistingZipCode_ShouldSucceedAndReturnNotFoundAddress()
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
        apiResponse.Exception.Should().BeNull();
        apiResponse.Success.Should().BeTrue();
        apiResponse.OperationCode.Should().Be(OperationCodes.Successful);
		apiResponse.Message.Should().BeEquivalentTo("Endereço não localizado.");
	}
}