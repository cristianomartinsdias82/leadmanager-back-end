using Application.Contracts.Persistence;
using Shared.RequestHandling;
using Shared.Results;

namespace Application.Features.Leads.Commands.UpdateLead;

internal sealed class UpdateLeadCommandHandler : ApplicationRequestHandler<UpdateLeadCommandRequest, UpdateLeadCommandResponse>
{
    private readonly ILeadManagerDbContext _dbContext;

    public UpdateLeadCommandHandler(ILeadManagerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async override Task<ApplicationResponse<UpdateLeadCommandResponse>> Handle(UpdateLeadCommandRequest request, CancellationToken cancellationToken)
    {
        var outdatedLead = await _dbContext.Leads.FindAsync(request.Id);
        if (outdatedLead is null)
            return ApplicationResponse<UpdateLeadCommandResponse>.Create(null!, message: "Lead não encontrado.");

        await outdatedLead.Atualizar(
            request.RazaoSocial,
            request.Cnpj,
            request.Cep,
            request.Endereco,
            request.Cidade,
            request.Bairro,
            request.Numero,
            request.Complemento);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return ApplicationResponse<UpdateLeadCommandResponse>.Create(new UpdateLeadCommandResponse());
    }
}