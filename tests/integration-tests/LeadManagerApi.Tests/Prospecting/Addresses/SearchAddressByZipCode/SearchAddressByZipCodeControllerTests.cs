using FluentAssertions;
using LeadManagerApi.Tests.Core.Factories;
using Shared.Results;
using System.Net;
using ViaCep.ServiceClient.Models;
using Xunit;
using static LeadManagerApi.Tests.Core.Factories.LeadManagerWebApplicationFactory;

namespace LeadManagerApi.Tests.Prospecting.Addresses.SearchAddressByZipCode;

public class SearchAddressByZipCodeControllerTests : IClassFixture<LeadManagerWebApplicationFactory>
{
    private readonly LeadManagerWebApplicationFactory _factory;
    private const string SearchAddressUri = $"{AddressesEndpoint}/search?cep=";

    public SearchAddressByZipCodeControllerTests(LeadManagerWebApplicationFactory factory)
    {
        _factory = factory;
    }

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
        // Arrange
        var httpClient = _factory.CreateHttpClient();

        // Act
        var response = await httpClient.GetAsync(string.Concat(SearchAddressUri, "04858040"));

        // Assert
        response.EnsureSuccessStatusCode();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var responseContent = await response.Content.ReadAsStringAsync();

        ApplicationResponse<Endereco> apiResponse = default!;
        Action action = () => { apiResponse = _factory.DeserializeFromJson<ApplicationResponse<Endereco>>(responseContent)!; };
        action.Should().NotThrow<Exception>();
        apiResponse.Exception.Should().BeNull();
        apiResponse.Success.Should().BeTrue();
        apiResponse.OperationCode.Should().Be(OperationCodes.Successful);
        apiResponse.Data.Should().NotBeNull();
        //other assertions..
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
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var responseContent = await response.Content.ReadAsStringAsync();

        ApplicationResponse<Endereco> apiResponse = default!;
        Action action = () => { apiResponse = _factory.DeserializeFromJson<ApplicationResponse<Endereco>>(responseContent)!; };
        action.Should().NotThrow<Exception>();
        apiResponse.Exception.Should().BeNull();
        apiResponse.Success.Should().BeTrue();
        apiResponse.OperationCode.Should().Be(OperationCodes.Successful);
        apiResponse.Data.Should().NotBeNull();
        //other assertions..
    }
}