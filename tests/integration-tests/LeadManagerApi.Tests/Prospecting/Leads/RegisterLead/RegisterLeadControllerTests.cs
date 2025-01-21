using Application.Core.Contracts.Persistence;
using FluentAssertions;
using LeadManagerApi.Tests.Core;
using LeadManagerApi.Tests.Core.Factories;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text.Json;
using Tests.Common.ObjectMothers.Application;
using Xunit;

namespace LeadManagerApi.Tests.Prospecting.Leads.RegisterLead;

public sealed class RegisterLeadControllerTests : SharedResourcesTestsBase
{
    public RegisterLeadControllerTests(
        LeadManagerWebApplicationFactory factory) : base(factory)
    {
        
    }

    [Fact]
    public async Task Post_RequestWithValidApiKeyHeaderAndWithValidData_ShouldSucceed()
    {
        //Arrange
        using var cts = new CancellationTokenSource();
        using var httpClient = _factory.CreateHttpClient();
        var newLeadContent = new StringContent(
                                    JsonSerializer.Serialize(RegisterLeadCommandRequestMother.XptoIncLeadRequest()),
                                    new MediaTypeHeaderValue(MediaTypeNames.Application.Json));

        //Act
        using var response = await httpClient.PostAsync(LeadsEndpoint, newLeadContent, cts.Token);

        //Basic asserts
        response.EnsureSuccessStatusCode();
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.IsSuccessStatusCode.Should().BeTrue();
        response.Headers.Location.Should().NotBeNull();

        //Extra asserts alternative 1 - Execute a get request in order to get the newly created lead
        var getNewLeadResponse = await httpClient.GetAsync(response.Headers.Location, cts.Token);
        getNewLeadResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        getNewLeadResponse.Should().NotBeNull();
        getNewLeadResponse.IsSuccessStatusCode.Should().BeTrue();

        //OR

        //Extra asserts alternative 2 - Get the newly created lead from a fresh instance of ILeadManagerDbContext
        var leadId = response.Headers.Location!.Segments[response.Headers.Location.Segments.Count() - 1];
        leadId.Should().NotBeEmpty();
        Guid.TryParse(leadId, out var id).Should().BeTrue();

        //You could do that but the response is 204 NO CONTENT!
        //var responseResult = await response.Content.ReadFromJsonAsync<ApplicationResponse<Guid>>(cts.Token);
        //responseResult.Should().NotBeNull();
        //responseResult!.Data.Should().NotBeEmpty();
        //var id = responseResult!.Data;

        using var appScope = _factory.Services.CreateAsyncScope();
        var db = appScope.ServiceProvider.GetRequiredService<ILeadManagerDbContext>();
        var newlyCreatedLead = db.Leads.FirstOrDefault(x => x.Id == id);
        newlyCreatedLead.Should().NotBeNull();
    }

    //[Fact]
    //public async Task Post_RequestWithValidApiKeyHeaderAndWithInvalidData_ShouldFail()
    //{
    //    //Arrange

    //    //Act

    //    //Assert
    //}


    //[Fact]
    //public async Task Post_RequestWithoutApiKeyHeader_ShouldFail()
    //{
    //    // Arrange
    //    var httpClient = _factory.CreateHttpClient(false);

    //    // Act
    //    var response = await httpClient.PostAsync(LeadsEndpoint);

    //    // Assert
    //    response.ReasonPhrase.Should().Contain(HttpStatusCode.Unauthorized.ToString(), because: "Api key header has not been set.");
    //    response.StatusCode.Should().Be(HttpStatusCode.Unauthorized, because: "Api key header has not been set.");
    //}

    //[Theory]
    //[InlineData("a", "b")]
    //[InlineData("X-Api-Key", "1341341")]
    //[InlineData("X-Api-Key", "")]
    //[InlineData("X-Lead-Manager-Api-Key", "12312312")]
    //[InlineData("X-Lead-Manager-Api-Key", "")]
    //[InlineData("c", "1341341")]
    //public async Task Post_RequestWithInvalidApiKeyHeader_ShouldFail(string headerName, string headerValue)
    //{
    //    // Arrange
    //    var httpClient = _factory.CreateHttpClient((headerName, headerValue));

    //    // Act
    //    var response = await httpClient.PostAsync(LeadsEndpoint);

    //    // Assert
    //    response.ReasonPhrase.Should().Contain("Unauthorized", because: "The informed Api key is invalid.");
    //    response.StatusCode.Should().Be(HttpStatusCode.Unauthorized, because: "The informed Api key is invalid.");
    //    Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    //}
}
