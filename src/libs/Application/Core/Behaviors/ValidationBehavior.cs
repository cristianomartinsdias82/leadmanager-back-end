using CrossCutting.Security.IAM;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Shared.Results;

namespace Application.Core.Behaviors;

internal sealed class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, ApplicationResponse<TResponse>>
    where TRequest : IRequest<ApplicationResponse<TResponse>>
{
    private readonly ILogger<ValidationBehavior<TRequest, TResponse>> _logger;
    private readonly IEnumerable<IValidator<TRequest>> _validators;
    private readonly IUserService _userService;

    public ValidationBehavior(
        IEnumerable<IValidator<TRequest>> validators,
        ILogger<ValidationBehavior<TRequest, TResponse>> logger,
        IUserService userService)
    {
        _logger = logger;
        _validators = validators;
        _userService = userService;
    }

    public async Task<ApplicationResponse<TResponse>> Handle(TRequest request, RequestHandlerDelegate<ApplicationResponse<TResponse>> next, CancellationToken cancellationToken)
    {
        if (_validators.Any())
        {
            var context = new ValidationContext<TRequest>(request);
            var validationResults = await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context, cancellationToken)));
            var failures = validationResults.SelectMany(r => r.Errors).Where(f => f is not null)
                            .Select(err => new Inconsistency(err.PropertyName, err.ErrorMessage))
                            .ToList();

            if (failures?.Any() ?? false)
            {
                _logger.LogInformation("Validation errors for request {@Request}: {@ValidationErrors}. User is: {UserEmail}", request, failures, _userService?.GetUserEmail() ?? "-");

                return ApplicationResponse<TResponse>.Create(
                    default!,
                    message: "Ocorreu um ou mais erros de validação. Por favor, corrija-o(s) e tente novamente.",
                    operationCode: OperationCodes.ValidationFailure,
                    inconsistencies: failures.ToArray());
            }
        }

        return await next();
    }
}