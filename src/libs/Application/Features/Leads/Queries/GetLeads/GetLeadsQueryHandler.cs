using Application.Contracts.Persistence;
using Application.Features.Leads.Queries.Shared;
using Microsoft.EntityFrameworkCore;
using Shared.RequestHandling;
using Shared.Results;

namespace Application.Features.Leads.Queries.GetLeads;

internal sealed class GetLeadsQueryHandler : ApplicationRequestHandler<GetLeadsQueryRequest, IEnumerable<LeadDto>>
{
    private readonly ILeadManagerDbContext _dbContext;

    public GetLeadsQueryHandler(ILeadManagerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async override Task<ApplicationResponse<IEnumerable<LeadDto>>> Handle(GetLeadsQueryRequest request, CancellationToken cancellationToken)
    {
        var leads = await _dbContext.Leads.ToListAsync(cancellationToken);

        var leadDtos = leads.Select(lead => new LeadDto
        (
            lead.Id,
            lead.Cnpj,
            lead.RazaoSocial,
            lead.Cep,
            lead.Logradouro,
            lead.Bairro,
            lead.Cidade,
            lead.Estado,
            lead.Numero,
            lead.Complemento
        )).ToList();

        return ApplicationResponse<IEnumerable<LeadDto>>.Create(leadDtos);
    }
}