using FluentValidation;

namespace Application.Prospecting.Leads.Commands.BulkRemoveLeadsFiles;

public sealed class BulkRemoveLeadsFilesCommandRequestValidator : AbstractValidator<BulkRemoveLeadsFilesCommandRequest>
{
	private const string NecessarioInformarAoMenos1Arquivo = "É necessário informar ao menos 1 arquivo.";
	private const string NaoPodeSerNulo = "O argumento Ids não pode ser nulo.";

	public BulkRemoveLeadsFilesCommandRequestValidator()
    {
        RuleFor(leadsFiles => leadsFiles.Ids)
            .NotNull()
            .WithMessage(NaoPodeSerNulo);

        When(leadsFiles => leadsFiles is not null, () =>
        {
			RuleFor(leadsFiles => leadsFiles.Ids.Any())
			.Equal(true)
			.WithMessage(NecessarioInformarAoMenos1Arquivo);
		});            
    }
}