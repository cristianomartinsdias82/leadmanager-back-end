namespace LeadManagerApi.Tests.Core.Extensions;

internal static class HttpClientExtensions
{
	//https://github.com/dotnet/runtime/issues/41473
	/// <summary>
	/// Sends a http DELETE request with data in the body
	/// </summary>
	public static async Task<HttpResponseMessage> DeleteAsync(
		this HttpClient httpClient,
		string requestUri,
		HttpContent content = default!,
		CancellationToken cancellationToken = default,
		params (string Name, string Value)[] httpHeaders)
	{
		var httpRequest = new HttpRequestMessage(HttpMethod.Delete, requestUri) { Content = content };
		if (httpHeaders is not null)
			foreach (var (Name, Value) in httpHeaders)
				httpRequest
					.Headers
					.TryAddWithoutValidation(Name, Value);

		return await httpClient.SendAsync(httpRequest, cancellationToken);
	}
}
