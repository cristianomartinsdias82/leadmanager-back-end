using Shared.Results;

namespace Shared.ApplicationOperationRules;

public abstract class ApplicationOperatingRule : IApplicationOperatingRule
{
	public virtual Inconsistency? Apply()
		=> ApplyAsync().GetAwaiter().GetResult();
	public abstract Task<Inconsistency?> ApplyAsync(CancellationToken cancellationToken = default);
}