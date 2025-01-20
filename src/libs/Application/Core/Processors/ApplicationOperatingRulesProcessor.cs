using CrossCutting.Security.IAM;
using MediatR.Pipeline;
using Shared.ApplicationOperationRules;
using Shared.Results;
using System.Collections.Concurrent;

namespace Application.Core.Processors;

public class ApplicationOperatingRulesProcessor<TRequest> : IRequestPreProcessor<TRequest> where TRequest : notnull
{
	private readonly IUserService _userService;
	private readonly IEnumerable<IApplicationOperatingRule> _operatingRules;

	public ApplicationOperatingRulesProcessor(
		IUserService userService,
		IEnumerable<IApplicationOperatingRule> operatingRules)
	{
		_userService = userService;
		_operatingRules = operatingRules;
	}

	//public Task Process(TRequest request, CancellationToken cancellationToken)
	//{
	//	if (!_userService.CurrentUserIsAdministrator)
	//		ApplyRules(_operatingRules);

	//	return Task.CompletedTask;
	//}

	//private void ApplyRules(IEnumerable<IApplicationOperatingRule> operatingRules)
	//{
	//	var inconsistencies = (operatingRules ?? Enumerable.Empty<IApplicationOperatingRule>()).Select(opRule => opRule.Apply());

	//	if (inconsistencies.Any(inc => inc is not null))
	//		throw new ApplicationOperatingRuleException(
	//				inconsistencies
	//					.Where(it => it is not null)
	//					.ToList(),
	//				string.Empty);
	//}

	public async Task Process(TRequest request, CancellationToken cancellationToken)
	{
		if (!_userService.CurrentUserIsAdministrator)
			await ApplyRulesAsync(_operatingRules, cancellationToken);
	}

	private async Task ApplyRulesAsync(IEnumerable<IApplicationOperatingRule> operatingRules, CancellationToken cancellationToken)
	{
		ConcurrentBag<Inconsistency?> inconsistencies = new();

		if (operatingRules?.Any() ?? false)
			await Task.WhenAll(operatingRules.Select(async it => inconsistencies.Add(await it.ApplyAsync(cancellationToken))));

		if (inconsistencies.Any(inc => inc is not null))
			throw new ApplicationOperatingRuleException(
					inconsistencies
						.Where(it => it is not null)
						.ToList(),
					string.Empty);
	}
}
