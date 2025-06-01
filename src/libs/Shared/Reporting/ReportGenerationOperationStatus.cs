namespace Shared.Reporting;

public enum ReportGenerationOperationStatus : int
{
	Pending = 1,
	Processing = 2,
	Successful = 3,
	SuccessfulWithRetries = 4,
	Failed = 5
}
