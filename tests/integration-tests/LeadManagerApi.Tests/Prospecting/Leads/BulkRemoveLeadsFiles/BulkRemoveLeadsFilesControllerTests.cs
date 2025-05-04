using FluentAssertions;
using LeadManagerApi.Tests.Core;
using LeadManagerApi.Tests.Core.Extensions;
using LeadManagerApi.Tests.Core.Factories;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text.Json;
using Tests.Common.ObjectMothers.Application;
using Xunit;

namespace LeadManagerApi.Tests.Prospecting.Leads.BulkRemoveLeadsFiles;

public sealed class BulkRemoveLeadsFilesControllerTests : SharedResourcesTestsBase
{
	private const string BulkRemoveLeadsFilesEndpoint = $"{LeadsEndpoint}/uploaded-files";

	public BulkRemoveLeadsFilesControllerTests(
		LeadManagerWebApplicationFactory factory) : base(factory) { }

	[Fact]
	public async Task Request_ExistingFiles_ReturnsSuccessful()
	{
		//Arrange
		using var cts = new CancellationTokenSource();
		using var httpClient = _factory.CreateHttpClient();
		var content = new StringContent(
									JsonSerializer.Serialize(BulkRemoveLeadsFilesCommandRequestMother.Default()),
									new MediaTypeHeaderValue(MediaTypeNames.Application.Json));

		//Act
		var response = await httpClient.DeleteAsync(
			BulkRemoveLeadsFilesEndpoint,
			content: content,
			cancellationToken: cts.Token,
			httpHeaders: [_factory.GetLeadManagerApiKeyNameValueHeader()]);

		//Assert
		response.EnsureSuccessStatusCode();
		response.StatusCode.Should().Be(HttpStatusCode.NoContent);
		response.IsSuccessStatusCode.Should().BeTrue();
	}
}