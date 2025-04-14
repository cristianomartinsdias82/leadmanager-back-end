using Application.Core.Contracts.Repository.Prospecting;
using MediatR;
using Shared.RequestHandling;
using Shared.Results;

namespace Application.Prospecting.Leads.Queries.ExistsLead;

internal sealed class ExistsLeadQueryRequestHandler : ApplicationRequestHandler<ExistsLeadQueryRequest, bool>
{
    private readonly ILeadRepository _leadRepository;

    public ExistsLeadQueryRequestHandler(
        IMediator mediator,
        ILeadRepository leadRepository) : base(mediator, default!)
    {
        _leadRepository = leadRepository;
    }

    public override async Task<ApplicationResponse<bool>> Handle(ExistsLeadQueryRequest request, CancellationToken cancellationToken)
    {
        var searchTerm = request.SearchTerm!.Trim();

        var leadExists = await _leadRepository
                                .ExistsAsync(ld => (!request.LeadId.HasValue || ld.Id != request.LeadId) &&
                                                   (ld.Cnpj == searchTerm || ld.RazaoSocial == searchTerm), cancellationToken);

        return ApplicationResponse<bool>.Create(leadExists);
    }
}