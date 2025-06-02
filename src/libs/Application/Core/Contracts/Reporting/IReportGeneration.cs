using Shared.DataQuerying;
using Shared.Results;

namespace Application.Core.Contracts.Reporting;

public interface IReportGeneration
{
	Task<ApplicationResponse<PersistableData>> GenerateAsync(
		QueryOptions? queryOptions,
		CancellationToken cancellationToken = default);
}
