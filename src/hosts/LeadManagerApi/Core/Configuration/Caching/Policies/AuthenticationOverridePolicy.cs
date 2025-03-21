//Keep it commented out for demonstration purposes of how to apply in projects.
//using Microsoft.AspNetCore.OutputCaching;
//using Microsoft.Extensions.Primitives;

////https://learn.microsoft.com/en-us/aspnet/core/performance/caching/output?view=aspnetcore-9.0

//namespace LeadManagerApi.Core.Configuration.Caching.Policies;

//public sealed class AuthenticationOverridePolicy : IOutputCachePolicy
//{
//	public static readonly AuthenticationOverridePolicy Instance = new();

//	private AuthenticationOverridePolicy() { }

//	ValueTask IOutputCachePolicy.CacheRequestAsync(
//		OutputCacheContext context,
//		CancellationToken cancellationToken)
//	{
//		var attemptOutputCaching = AttemptOutputCaching(context);
//		context.EnableOutputCaching = true;
//		context.AllowCacheLookup = attemptOutputCaching;
//		context.AllowCacheStorage = attemptOutputCaching;
//		context.AllowLocking = true;

//		// Assign a tag for enabling eviction
//		context.Tags.Add(LeadManagerApiCachingConfiguration.Policies.Get.Tag);

//		// Vary by any query by default
//		context.CacheVaryByRules.QueryKeys = new StringValues(["search", "page", "pageSize", "sortColumn", "sortDirection"]);

//		// Expiration time
//		context.ResponseExpirationTimeSpan = TimeSpan.FromMinutes(5);

//		return ValueTask.CompletedTask;
//	}

//	ValueTask IOutputCachePolicy.ServeFromCacheAsync
//		(OutputCacheContext context, CancellationToken cancellationToken)
//	{
//		return ValueTask.CompletedTask;
//	}

//	ValueTask IOutputCachePolicy.ServeResponseAsync
//		(OutputCacheContext context, CancellationToken cancellationToken)
//	{
//		//var response = context.HttpContext.Response;

//		//// Verify existence of cookie headers
//		//if (!StringValues.IsNullOrEmpty(response.Headers.SetCookie))
//		//{
//		//	context.AllowCacheStorage = false;
//		//	return ValueTask.CompletedTask;
//		//}

//		//// Check response code
//		//if (response.StatusCode != StatusCodes.Status200OK &&
//		//	response.StatusCode != StatusCodes.Status301MovedPermanently)
//		//{
//		//	context.AllowCacheStorage = false;
//		//	return ValueTask.CompletedTask;
//		//}

//		return ValueTask.CompletedTask;
//	}

//	private static bool AttemptOutputCaching(OutputCacheContext context)
//	{
//		// Check if the current request fulfills the requirements
//		// to be cached
//		var request = context.HttpContext.Request;

//		// Verify the method
//		if (!HttpMethods.IsGet(request.Method) &&
//			!HttpMethods.IsHead(request.Method) &&
//			!HttpMethods.IsPost(request.Method))
//		{
//			return false;
//		}

//		//By commenting the code below, authenticated responses are cached!
//		// Verify existence of authorization headers
//		//if (!StringValues.IsNullOrEmpty(request.Headers.Authorization) ||
//		//	request.HttpContext.User?.Identity?.IsAuthenticated == true)
//		//{
//		//	return false;
//		//}

//		return true;
//	}
//}
