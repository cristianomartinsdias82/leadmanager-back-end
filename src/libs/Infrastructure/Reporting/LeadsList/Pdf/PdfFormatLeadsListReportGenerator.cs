using Application.Core.Contracts.Reporting;
using Shared.DataQuerying;
using Shared.Results;

namespace Infrastructure.Reporting.LeadsList.Pdf;

public class PdfFormatLeadsListReportGenerator : LeadsListReportGenerator
{
	public override async Task<ApplicationResponse<PersistableData>> GenerateAsync(QueryOptions? queryOptions, CancellationToken cancellationToken = default)
	{
		await Task.Delay(0, cancellationToken);
		return ApplicationResponse<PersistableData>
					.Create(new(Array.Empty<byte>(), 0, "application/pdf", default));

		////Error simulation
		//return ApplicationResponse<PersistableData>
		//			.Create(
		//				default!,
		//				"Ugly error in the system!",
		//				OperationCodes.Error,
		//				exception: default,
		//				inconsistencies: [new Inconsistency("X", "Y")]);
	}
}
