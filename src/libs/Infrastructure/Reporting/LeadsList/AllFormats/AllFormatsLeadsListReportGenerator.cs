using Application.Core.Contracts.Reporting;
using Shared.DataQuerying;
using Shared.Results;

namespace Infrastructure.Reporting.LeadsList.AllFormats;

public class AllFormatsLeadsListReportGenerator : LeadsListReportGenerator
{
	public override Task<ApplicationResponse<PersistableData>> GenerateAsync(QueryOptions? queryOptions, CancellationToken cancellationToken = default)
	{
		throw new NotImplementedException();
	}
}
