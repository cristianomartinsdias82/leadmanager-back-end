using Shared.ApplicationOperationRules;
using Shared.Results;

namespace Application.Core.OperatingRules;

internal sealed class WeekDaysOnlyOperatingRule : ApplicationOperatingRule
{
	private readonly TimeProvider _timeProvider;

	public WeekDaysOnlyOperatingRule(TimeProvider timeProvider)
	{
		_timeProvider = timeProvider;
	}

	public override Task<Inconsistency?> ApplyAsync(CancellationToken cancellationToken = default)
	{
		var now = _timeProvider.GetLocalNow();
		if (new DayOfWeek[] { DayOfWeek.Saturday, DayOfWeek.Sunday }.Contains(now.DayOfWeek))
			return Task.FromResult<Inconsistency?>(new("Regras para funcionamento da aplicação", "não é permitido utilizar a aplicação aos fins de semana."));

		return Task.FromResult<Inconsistency?>(default);
	}
}
