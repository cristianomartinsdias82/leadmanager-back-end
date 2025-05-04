using Application.Core.Contracts.Repository.Prospecting;
using MediatR;
using Shared.DataPagination;
using Shared.RequestHandling;
using Shared.Results;

namespace Application.Prospecting.Leads.Queries.GetUploadedLeadsFiles;

internal sealed class GetUploadedLeadsFilesQueryRequestHandler : ApplicationRequestHandler<GetUploadedLeadsFilesQueryRequest, PagedList<UploadedLeadsFileDto>>
{
    private readonly ILeadRepository _leadRepository;

    public GetUploadedLeadsFilesQueryRequestHandler(
        IMediator mediator,
		ILeadRepository leadRepository)
         : base(mediator, default!)
    {
        _leadRepository = leadRepository;
	}

    public async override Task<ApplicationResponse<PagedList<UploadedLeadsFileDto>>> Handle(
		GetUploadedLeadsFilesQueryRequest request,
        CancellationToken cancellationToken)
    {
        var pagedUploadedLeadsFiles = await _leadRepository.GetLeadsFilesAsync(
            request.PaginationOptions,
            cancellationToken);

		return ApplicationResponse<PagedList<UploadedLeadsFileDto>>
                .Create(pagedUploadedLeadsFiles.MapPage(UploadedLeadsFileMapper.ToDto));
    }
}
