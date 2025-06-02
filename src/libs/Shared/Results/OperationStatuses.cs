namespace Shared.Results;

public enum OperationStatuses : int
{
	Pending = 1,
	ReadyForProcessing = 2,
	Processing = 3,
	Successful = 4,
	SuccessfulWithRetries = 5,
	Failed = 6
}