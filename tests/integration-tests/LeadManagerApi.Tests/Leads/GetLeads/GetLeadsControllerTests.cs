using Application.Features.Leads.Shared;
using FluentAssertions;
using LeadManagerApi.Tests.Common.Factories;
using Shared.DataPagination;
using Shared.Results;
using System.Net;
using Xunit;
using static LeadManagerApi.Tests.Common.Factories.LeadManagerWebApplicationFactory;

namespace LeadManagerApi.Tests.Leads.GetLeads;

public class GetLeadsControllerTests : IClassFixture<LeadManagerWebApplicationFactory>
{
    private readonly LeadManagerWebApplicationFactory _factory;

    public GetLeadsControllerTests(LeadManagerWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Get_RequestWithoutApiKeyHeader_ShouldFail()
    {
        // Arrange
        var httpClient = _factory.CreateHttpClient(false);

        // Act
        var response = await httpClient.GetAsync(LeadsEndpoint);

        // Assert
        response.ReasonPhrase.Should().Contain(HttpStatusCode.Unauthorized.ToString(), because: "Api key header has not been set.");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized, because: "Api key header has not been set.");
    }

    [Theory]
    [InlineData("a", "b")]
    [InlineData("X-Api-Key", "1341341")]
    [InlineData("X-Api-Key", "")]
    [InlineData("X-Lead-Manager-Api-Key", "12312312")]
    [InlineData("X-Lead-Manager-Api-Key", "")]
    [InlineData("c", "1341341")]
    public async Task Get_RequestWithInvalidApiKeyHeader_ShouldFail(string headerName, string headerValue)
    {
        // Arrange
        var httpClient = _factory.CreateHttpClient((headerName, headerValue));

        // Act
        var response = await httpClient.GetAsync(LeadsEndpoint);

        // Assert
        response.ReasonPhrase.Should().Contain("Unauthorized", because: "The informed Api key is invalid.");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized, because: "The informed Api key is invalid.");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Get_RequestWithValidApiKeyHeader_ShouldSucceed()
    {
        // Arrange
        //await _factory.UsingContextAsync(dbContext =>
        //{
        //    dbContext.Leads.AddRangeAsync(
        //        LeadMother.Leads()
        //    );

        //    dbContext.SaveChangesAsync();

        //    return Task.CompletedTask;
        //});

        var httpClient = _factory.CreateHttpClient();

        // Act
        var response = await httpClient.GetAsync(LeadsEndpoint);

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
        apiResponse.Data.Items.Should().NotBeNull().And.HaveCountGreaterThan(0);
    }
}