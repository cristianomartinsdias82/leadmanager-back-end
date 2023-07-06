using FluentValidation;

namespace Shared.Validation;

public static class FluentValidationExtensions
{
    public static IRuleBuilderOptions<T, TProperty> IsValidCnpj<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder, bool addFailure = false)
        => ruleBuilder.Must((instance, value, context) =>
        {
            if (!CnpjValidator.IsValidCnpj(value?.ToString() ?? string.Empty))
            {
                if (addFailure)
                    context.AddFailure("Cnpj inválido.");

                return false;
            }

            return true;
        });
}