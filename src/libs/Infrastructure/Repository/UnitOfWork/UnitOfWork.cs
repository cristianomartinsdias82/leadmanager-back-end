using Application.Core.Contracts.Persistence;
using Application.Core.Contracts.Repository.UnitOfWork;

namespace Infrastructure.Repository.UnitOfWork;

internal sealed class UnitOfWork : IUnitOfWork
{
    private readonly ILeadManagerDbContext _leadManagerDbContext;
	private readonly List<Action<CancellationToken>> _postSuccessfulCommitActions;

	public UnitOfWork(
		ILeadManagerDbContext leadManagerDbContext)
    {
        _leadManagerDbContext = leadManagerDbContext;
		_postSuccessfulCommitActions = [];
	}

	public void AddNonObtrusiveCommitSuccessfulPostAction(Action<CancellationToken> operation)
	{
		ArgumentNullException.ThrowIfNull(operation, nameof(operation));

		_postSuccessfulCommitActions.Add(operation);
	}

	public async Task<int> CommitAsync(CancellationToken cancellationToken = default)
	{
		var rowsAffected = await _leadManagerDbContext.SaveChangesAsync(cancellationToken);
		if (rowsAffected > 0 && _postSuccessfulCommitActions.Count > 0)
			_postSuccessfulCommitActions.ForEach(action => { try { action(cancellationToken); } catch { } });

		_postSuccessfulCommitActions.Clear();

		return rowsAffected;
	}
}
