using Application.Contracts.Persistence;
using Core.Entities;
using Shared.RequestHandling;
using Shared.Results;

namespace Application.Features.Leads.Commands.RegisterLead;

internal sealed class RegisterLeadCommandHandler : ApplicationRequestHandler<RegisterLeadCommandRequest, RegisterLeadCommandResponse>
{
    private readonly ILeadManagerDbContext _dbContext;

    public RegisterLeadCommandHandler(ILeadManagerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async override Task<ApplicationResponse<RegisterLeadCommandResponse>> Handle(RegisterLeadCommandRequest request, CancellationToken cancellationToken)
    {
        var lead = new Lead(
            request.Cnpj,
            request.RazaoSocial,
            request.Cep,
            request.Endereco,
            request.Bairro,
            request.Cidade,
            request.Estado,
            request.Numero,
            request.Complemento
        );

        await _dbContext.Leads.AddAsync(lead);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return ApplicationResponse<RegisterLeadCommandResponse>.Create(new(lead.Id));
    }
}