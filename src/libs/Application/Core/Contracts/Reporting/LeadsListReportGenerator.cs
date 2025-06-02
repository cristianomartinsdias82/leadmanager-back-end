using Shared.DataQuerying;
using Shared.Results;

namespace Application.Core.Contracts.Reporting;

public abstract class LeadsListReportGenerator : ILeadsListReportGeneration
{
	public abstract Task<ApplicationResponse<PersistableData>> GenerateAsync(
		QueryOptions? queryOptions,
		CancellationToken cancellationToken = default);
}
