using FluentAssertions;
using LeadManagerApi.Tests.Core.Factories;
using LeadManagerApi.Tests.Core;
using System.Net;
using Xunit;
using Application.Security.Auditing.Logins.Commands.LogUserLoggedInEntry;

namespace LeadManagerApi.Tests.Security.Auditing.Logins.LogUserLoggedInEntry;

public class LogUserLoggedInEntryControllerTests : SharedResourcesTestsBase
{
    public LogUserLoggedInEntryControllerTests(
        LeadManagerWebApplicationFactory factory) : base(factory) { }

    [Fact]
    public async Task Post_ApiKeyHeaderNotSet_ShouldFail()
    {
        // Arrange
        var httpClient = _factory.CreateHttpClient(false);

        // Act
        var response = await httpClient.PostAsync(UserLoginAuditingEndpoint, new StringContent(string.Empty));

        // Assert
        response.ReasonPhrase.Should().Contain(HttpStatusCode.Unauthorized.ToString(), because: "Api key header has not been set.");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized, because: "Api key header has not been set.");
    }

    [Fact]
    public async Task LogUserLoggedInEntry_ShouldSucceed_And_ReturnResponse()
    {
        // Arrange
        var httpClient = _factory.CreateHttpClient();
        var request = new LogUserLoggedInEntryCommandRequest();

        //If you ever need to send some data in the post/update/patch operations, use the following
        //class and pass an instance as an argument to the httpclient's post method
        //var htmlContent = new StringContent(
        //                        JsonSerializer.Serialize(request),
        //                        Encoding.UTF8,
        //                        MediaTypeNames.Application.Json);

        // Act
        var response = await httpClient.PostAsync(UserLoginAuditingEndpoint, null/*htmlContent*/);

        // Assert
        response.EnsureSuccessStatusCode();
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        //Use the statements down below in case the api returns a Response
        //var responseContent = await response.Content.ReadAsStringAsync();
        //ApplicationResponse<LogUserLoggedInEntryCommandResponse> apiResponse = default!;
        //Action action = () => { apiResponse = _factory.DeserializeFromJson<ApplicationResponse<LogUserLoggedInEntryCommandResponse>>(responseContent)!; };
        //action.Should().NotThrow<Exception>();
        //apiResponse.Exception.Should().BeNull();
        //apiResponse.Success.Should().BeTrue();
        //apiResponse.OperationCode.Should().Be(OperationCodes.Successful);
        //apiResponse.Data.Should()
        //            .NotBeNull()
        //            .And
        //            .BeOfType<LogUserLoggedInEntryCommandResponse>();
        
        //other assertions..
    }
}
