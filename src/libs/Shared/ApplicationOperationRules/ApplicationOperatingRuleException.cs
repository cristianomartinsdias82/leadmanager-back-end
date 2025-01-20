using Shared.Results;

namespace Shared.ApplicationOperationRules;

public sealed class ApplicationOperatingRuleException : ApplicationException
{
	private readonly List<Inconsistency?> _ruleViolations;

	public ApplicationOperatingRuleException(List<Inconsistency?> ruleViolations, string? message) : this(ruleViolations, message, default)
	{
		
	}

	public ApplicationOperatingRuleException(List<Inconsistency?> ruleViolations, string? message, Exception? innerException) : base(message, innerException)
	{
		_ruleViolations = ruleViolations;
	}

	public IReadOnlyCollection<Inconsistency?> RuleViolations
		=> _ruleViolations
			.ToList()
			.AsReadOnly();
}