using Shared.DataQuerying;
using Shared.Results;

namespace Application.Core.Contracts.Reporting;

public abstract class UsersActionsReportGenerator : IUsersActionsReportGeneration
{
	public abstract Task<ApplicationResponse<PersistableData>> GenerateAsync(
		QueryOptions? queryOptions,
		CancellationToken cancellationToken = default);
}
