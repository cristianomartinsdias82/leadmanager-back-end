using Application.Contracts.Persistence;
using MediatR;
using Shared.RequestHandling;
using Shared.Results;

namespace Application.Features.Leads.Commands.RemoveLead;

internal sealed class RemoveLeadCommandHandler : ApplicationRequestHandler<RemoveLeadCommandRequest, RemoveLeadCommandResponse>
{
    private readonly ILeadManagerDbContext _dbContext;

    public RemoveLeadCommandHandler(
        IMediator mediator,
        ILeadManagerDbContext dbContext) : base(mediator, default!)
    {
        _dbContext = dbContext;
    }

    public async override Task<ApplicationResponse<RemoveLeadCommandResponse>> Handle(RemoveLeadCommandRequest request, CancellationToken cancellationToken)
    {
        var lead = await _dbContext.Leads.FindAsync(request.Id);
        if (lead is null)
            return ApplicationResponse<RemoveLeadCommandResponse>.Create(null!, message: "Lead não encontrado.");

        _dbContext.Leads.Remove(lead);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return ApplicationResponse<RemoveLeadCommandResponse>.Create(new RemoveLeadCommandResponse());
    }
}