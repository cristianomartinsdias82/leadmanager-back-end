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
		//Ideas: parameterize these constants either in appsettings files, yaml files or even in a database with cached values
		const int InitialBusinessTime = 7; 
		const int EndingBusinessTime = 19;

		var localNow = _timeProvider.GetLocalNow();
		if (localNow.Hour < InitialBusinessTime || localNow.Hour >= EndingBusinessTime)
			return Task.FromResult<Inconsistency?>(new("Regras para funcionamento da aplicação", $"não é permitido utilizar a aplicação fora do horário comercial (entre {InitialBusinessTime} e {EndingBusinessTime} horas)."));

		return Task.FromResult<Inconsistency?>(default);
	}
}
