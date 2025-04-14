using Application.Core.Contracts.Repository.Prospecting;
using Domain.Prospecting.Entities;
using MediatR;
using Shared.DataPagination;
using Shared.RequestHandling;
using Shared.Results;

namespace Application.Prospecting.Leads.Queries.GetLeads;

internal sealed class GetLeadsQueryRequestHandler : ApplicationRequestHandler<GetLeadsQueryRequest, PagedList<LeadDto>>
{
    private readonly ILeadRepository _leadRepository;

    public GetLeadsQueryRequestHandler(
        IMediator mediator,
        ILeadRepository leadRepository) : base(mediator, default!)
    {
        _leadRepository = leadRepository;
    }

    public async override Task<ApplicationResponse<PagedList<LeadDto>>> Handle(
        GetLeadsQueryRequest request,
        CancellationToken cancellationToken)
    {
        var pagedLeads = await _leadRepository.GetAsync(
            request.PaginationOptions,
            request.Search,
            cancellationToken);

        return ApplicationResponse<PagedList<LeadDto>>.Create(pagedLeads.MapPage(ld => ld.MapToDto()));
    }
}
