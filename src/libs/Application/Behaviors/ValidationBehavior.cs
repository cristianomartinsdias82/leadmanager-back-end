using FluentValidation;
using MediatR;
using Shared.Results;

namespace Application.Behaviors;

internal sealed class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, ApplicationResponse<TResponse>>
    where TRequest : IRequest<ApplicationResponse<TResponse>>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
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
                return ApplicationResponse<TResponse>.Create(
                    default!,
                    message: "Ocorreu um ou mais erros de validação. Por favor, corrija-o(s) e tente novamente.",
                    operationCode: OperationCodes.ValidationFailure,
                    inconsistencies: failures.ToArray());
        }

        return await next();
    }
}