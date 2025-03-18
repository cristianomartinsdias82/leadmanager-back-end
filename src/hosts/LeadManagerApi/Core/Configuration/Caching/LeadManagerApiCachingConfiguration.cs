namespace LeadManagerApi.Core.Configuration.Caching;

internal static class LeadManagerApiCachingConfiguration
{
	public static class Policies
	{
		public static class Get
		{
			public const string Name = "LeadManagerCachingPolicy";
			public const string Tag = "Leads";
		}
	}
}
