namespace Application.Core.Contracts.Repository.UnitOfWork;

public interface IUnitOfWork
{
	Task<int> CommitAsync(CancellationToken cancellationToken = default);

	void AddNonObtrusiveCommitSuccessfulPostAction(Action<CancellationToken> operation);
}