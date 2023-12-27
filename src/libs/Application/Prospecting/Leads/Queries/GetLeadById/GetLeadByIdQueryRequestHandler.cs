using Application.Core.Contracts.Repository.Prospecting;
using Domain.Prospecting.Entities;
using MediatR;
using Shared.RequestHandling;
using Shared.Results;

namespace Application.Prospecting.Leads.Queries.GetLeadById;

internal sealed class GetLeadByIdQueryRequestHandler : ApplicationRequestHandler<GetLeadByIdQueryRequest, LeadDto>
{
    private readonly ILeadRepository _leadRepository;

    public GetLeadByIdQueryRequestHandler(
        IMediator mediator,
        ILeadRepository leadRepository) : base(mediator, default!)
    {
        _leadRepository = leadRepository;
    }

    public override async Task<ApplicationResponse<LeadDto>> Handle(GetLeadByIdQueryRequest request, CancellationToken cancellationToken)
    {
        var lead = await _leadRepository.GetByIdAsync(request.Id, cancellationToken);
        if (lead is null)
            return ApplicationResponse<LeadDto>
                    .Create(default!,
                            message: "Lead não encontrado.",
                            operationCode: OperationCodes.NotFound);

        return ApplicationResponse<LeadDto>.Create(lead.MapToDto());
    }
}