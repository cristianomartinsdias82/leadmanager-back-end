using Application.Core.Contracts.Reporting;
using Shared.DataQuerying;
using Shared.Results;

namespace Infrastructure.Reporting.UsersActions.AllFormats;

public class AllFormatsUsersActionsReportGenerator : UsersActionsReportGenerator
{
	public override Task<ApplicationResponse<PersistableData>> GenerateAsync(QueryOptions? queryOptions, CancellationToken cancellationToken = default)
	{
		throw new NotImplementedException();
	}
}