
namespace LeadManager.BackendServices.Consumers.Common.Diagnostics;

public static class MessageConsumersDiagnostics
{
	public static class CommonConstants
	{
		public const string LeadProcessResult = "lead.process.result";
		public const string LeadProcessResultSucessful = "lead.process.result.successful";
	}

	public static class NewlyCreatedLeadsConstants
	{
		public const string ActivityName = "Newly created leads process";
		public const string Count = "lead.new.count";
		public const string LeadIds = "lead.new.ids";
	}

	public static class UpdatedLeadConstants
	{
		public const string ActivityName = "Updated lead process";
		public const string LeadId = "lead.updated.id";
	}

	public static class RemovedLeadConstants
	{
		public const string ActivityName = "Removed lead process";
		public const string LeadId = "lead.removed.id";
	}
}
