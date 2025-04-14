using Application.Core.Contracts.Repository.Prospecting;
using Domain.Prospecting.Entities;
using MediatR;
using Shared.RequestHandling;
using Shared.Results;

namespace Application.Prospecting.Leads.Queries.GetLeadById;

internal sealed class GetLeadByIdQueryRequestHandler : ApplicationRequestHandler<GetLeadByIdQueryRequest, LeadDto>
{
	private static readonly string Mensagem_LeadNaoEncontrado = "Código do Lead inválido ou os dados foram excluídos por outro usuário.";

	private readonly ILeadRepository _leadRepository;

    public GetLeadByIdQueryRequestHandler(
        IMediator mediator,
        ILeadRepository leadRepository) : base(mediator, default!)
    {
        _leadRepository = leadRepository;
    }

    public override async Task<ApplicationResponse<LeadDto>> Handle(GetLeadByIdQueryRequest request, CancellationToken cancellationToken)
    {
        var lead = await _leadRepository.GetByIdAsync(request.Id, cancellationToken: cancellationToken);
        if (lead is null)
            return ApplicationResponse<LeadDto>
                    .Create(default!,
                            message: Mensagem_LeadNaoEncontrado,
                            operationCode: OperationCodes.NotFound,
                            inconsistencies: new Inconsistency(string.Empty, Mensagem_LeadNaoEncontrado));

        return ApplicationResponse<LeadDto>.Create(lead.MapToDto());
    }
}