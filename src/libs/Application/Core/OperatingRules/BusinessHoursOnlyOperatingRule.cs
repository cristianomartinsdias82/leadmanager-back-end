using Shared.ApplicationOperationRules;
using Shared.Results;

namespace Application.Core.OperatingRules;

internal sealed class BusinessHoursOnlyOperatingRule : ApplicationOperatingRule
{
	private readonly TimeProvider _timeProvider;

	public BusinessHoursOnlyOperatingRule(TimeProvider timeProvider)
	{
		_timeProvider = timeProvider;
	}

	public override Task<Inconsistency?> ApplyAsync(CancellationToken cancellationToken = default)
	{
		const int MinHour = 7;
		const int MaxHour = 19;

		var now = _timeProvider.GetUtcNow();
		if (now.Hour < MinHour || now.Hour >= MaxHour)
			return Task.FromResult<Inconsistency?>(new("Regras para funcionamento da aplicação", $"não é permitido utilizar a aplicação fora do horário comercial (entre {MinHour} e {MaxHour} horas)."));

		return Task.FromResult<Inconsistency?>(default);
	}
}
