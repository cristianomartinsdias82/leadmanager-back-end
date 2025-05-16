using FluentValidation;

namespace Application.Security.Auditing.Queries.ListUsersActions
{
	public sealed class ListUsersActionsQueryRequestValidator : AbstractValidator<ListUsersActionsQueryRequest>
	{
		private const string DataInicioMenorIgualDataTermino = "Campo data de início deve ser menor ou igual a data de término.";

		public ListUsersActionsQueryRequestValidator()
		{
			RuleFor(request => request)
				.NotNull()
				.WithMessage("Requisição inválida.");

			When(request => request.QueryOptions is not null, () =>
			{
				When(args => args.QueryOptions.StartDate.HasValue &&
							 args.QueryOptions.EndDate.HasValue, () =>
							 {
								 RuleFor(x => x)
									.Must(req => req.QueryOptions.StartDate <= req.QueryOptions.EndDate)
									.WithMessage(DataInicioMenorIgualDataTermino);
							 });
			});
		}
	}
}
