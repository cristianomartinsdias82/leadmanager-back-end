using Shared.Results;

namespace Shared.ApplicationOperationRules;

public interface IApplicationOperatingRule
{
	Inconsistency? Apply();
	Task<Inconsistency?> ApplyAsync(CancellationToken cancellationToken = default);
}
