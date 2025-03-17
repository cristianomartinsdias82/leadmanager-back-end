using Domain.Prospecting.Entities;
using FluentAssertions;
using LeadManagerApi.Tests.Core;
using LeadManagerApi.Tests.Core.Factories;
using Shared.DataPagination;
using Shared.Results;
using System.Net;
using Xunit;

namespace LeadManagerApi.Tests.Prospecting.Leads.GetLeads;

public class GetLeadsControllerTests : SharedResourcesTestsBase
{
    private const string CorrectApiKeyRequestHeaderName = "LeadManager-Api-Key";

	public GetLeadsControllerTests(LeadManagerWebApplicationFactory factory) : base(factory) {}

    [Fact]
    public async Task Get_RequestWithoutApiKeyHeader_ReturnsUnauthorized()
    {
        // Arrange
        using var httpClient = _factory.CreateHttpClient(false);

        // Act
        using var response = await httpClient.GetAsync(LeadsEndpoint);

        // Assert
        response.ReasonPhrase.Should().Contain(HttpStatusCode.Unauthorized.ToString(), because: "Api key header has not been set.");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized, because: "Api key header has not been set.");
    }

	[Theory]
    [MemberData(nameof(InvalidApiKeyHeadersAndValues))]
    public async Task Get_RequestWithInvalidApiKeyHeaderAndOrValue_ReturnsUnauthorized(string headerName, string headerValue)
    {
        // Arrange
        using var httpClient = _factory.CreateHttpClient((headerName, headerValue));

		// Act
		using var response = await httpClient.GetAsync(LeadsEndpoint);

        // Assert
        response.ReasonPhrase.Should().Contain("Unauthorized", because: "The informed Api key is invalid.");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized, because: "The informed Api key is invalid.");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Get_RequestWithValidApiKeyHeader_ReturnsOk()
    {
        // Arrange
        using var httpClient = _factory.CreateHttpClient();

        // Act
        using var response = await httpClient.GetAsync(LeadsEndpoint);

        // Assert
        response.EnsureSuccessStatusCode();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var responseContent = await response.Content.ReadAsStringAsync();

        ApplicationResponse<PagedList<LeadDto>> apiResponse = default!;
        Action action = () =>
        {
            apiResponse = _factory.DeserializeFromJson<ApplicationResponse<PagedList<LeadDto>>>(responseContent)!;
        };

        action.Should().NotThrow<Exception>();
        apiResponse.Exception.Should().BeNull();
        apiResponse.Success.Should().BeTrue();
        apiResponse.OperationCode.Should().Be(OperationCodes.Successful);
        apiResponse.Data.Should().NotBeNull();
    }

	[Fact(Skip = "Why does this test succeeds when in debug mode but fails when it's not?")]
	public async Task Get_RequestWithTimeWindowRule_Enabled_ReturnsResponseWithErrorCode()
	{
		// Arrange
		using var httpClient = _factory.CreateHttpClient(
                                    true,
                                    ("Time-window-rule-violation-test", "1"));

		// Act
		using var response = await httpClient.GetAsync(LeadsEndpoint);

		// Assert
		response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
		response.IsSuccessStatusCode.Should().BeFalse();
		var responseContent = await response.Content.ReadAsStringAsync();

        ApplicationResponse<object> apiResponse = default!;
        Action action = () =>
        {
            apiResponse = _factory.DeserializeFromJson<ApplicationResponse<object>>(responseContent)!;
        };

        action.Should().NotThrow<Exception>();
        apiResponse.Exception.Should().NotBeNull();
        apiResponse.Success.Should().BeFalse();
        apiResponse.OperationCode.Should().Be(OperationCodes.Error);
        apiResponse.Inconsistencies.Should().NotBeNull();
        apiResponse.Inconsistencies!.Any(inc => inc.Description.Contains("não é permitido utilizar a aplicação fora do horário comercial"))
                                    .Should().BeTrue();
	}

	[Fact(Skip = "Why does this test succeeds when in debug mode but fails when it's not?")]
	public async Task Get_RequestWithDayRule_Enabled_ReturnsResponseWithErrorCode()
	{
		// Arrange
		using var httpClient = _factory.CreateHttpClient(
                                    true,
                                    ("Day-rule-violation-test", "1"));

		// Act
		using var response = await httpClient.GetAsync(LeadsEndpoint);

		// Assert
		response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
		response.IsSuccessStatusCode.Should().BeFalse();
		var responseContent = await response.Content.ReadAsStringAsync();

		ApplicationResponse<object> apiResponse = default!;
		Action action = () =>
		{
			apiResponse = _factory.DeserializeFromJson<ApplicationResponse<object>>(responseContent)!;
		};

		action.Should().NotThrow<Exception>();
		apiResponse.Exception.Should().NotBeNull();
		apiResponse.Success.Should().BeFalse();
		apiResponse.OperationCode.Should().Be(OperationCodes.Error);
		apiResponse.Inconsistencies!.Any(inc => inc.Description.Contains("não é permitido utilizar a aplicação aos fins de semana"))
									.Should().BeTrue();
	}

	public static IEnumerable<object[]> InvalidApiKeyHeadersAndValues()
	{
		yield return new object[] { "a", "b" };
		yield return new object[] { "X-Api-Key", "1341341" };
		yield return new object[] { "X-Api-Key", string.Empty };
		yield return new object[] { CorrectApiKeyRequestHeaderName, "12312312" };
		yield return new object[] { CorrectApiKeyRequestHeaderName, string.Empty };
		yield return new object[] { "c", "1341341" };
	}
}