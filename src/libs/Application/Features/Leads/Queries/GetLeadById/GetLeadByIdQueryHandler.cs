using Application.Contracts.Persistence;
using Application.Features.Leads.Shared;
using MediatR;
using Shared.RequestHandling;
using Shared.Results;

namespace Application.Features.Leads.Queries.GetLeadById;

internal sealed class GetLeadByIdQueryHandler : ApplicationRequestHandler<GetLeadByIdQueryRequest, LeadDto>
{
    private readonly ILeadManagerDbContext _dbContext;

    public GetLeadByIdQueryHandler(
        IMediator mediator,
        ILeadManagerDbContext dbContext) : base(mediator, default!)
    {
        _dbContext = dbContext;
    }

    public override async Task<ApplicationResponse<LeadDto>> Handle(GetLeadByIdQueryRequest request, CancellationToken cancellationToken)
    {
        var lead = await _dbContext.Leads.FindAsync(request.Id);
        if (lead is null)
            return ApplicationResponse<LeadDto>
                    .Create(default!,
                            message: "Lead não encontrado.",
                            operationCode: OperationCodes.NotFound);

        var leadDto = new LeadDto
        {
            Id = lead.Id,
            Cnpj = lead.Cnpj,
            RazaoSocial = lead.RazaoSocial,
            Cep = lead.Cep,
            Endereco = lead.Logradouro,
            Bairro = lead.Bairro,
            Cidade = lead.Cidade,
            Estado = lead.Estado,
            Numero = lead.Numero,
            Complemento = lead.Complemento
        };

        return ApplicationResponse<LeadDto>.Create(leadDto);
    }
}