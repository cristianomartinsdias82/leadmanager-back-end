using Application.Core.Contracts.Repository.Prospecting;
using CrossCutting.FileStorage;
using MediatR;
using Shared.RequestHandling;
using Shared.Results;

namespace Application.Prospecting.Leads.Queries.DownloadLeadsFile;

internal sealed class DownloadLeadsFileQueryRequestHandler : ApplicationRequestHandler<DownloadLeadsFileQueryRequest, DownloadLeadsFileDto?>
{
	private readonly ILeadRepository _leadRepository;
	private readonly IFileStorageProvider _fileStorageProvider;

	public DownloadLeadsFileQueryRequestHandler(
        IMediator mediator,
        ILeadRepository leadRepository,
		IFileStorageProvider fileStorageProvider)
         : base(mediator, default!)
    {
		_leadRepository = leadRepository;
		_fileStorageProvider = fileStorageProvider;
	}

    public async override Task<ApplicationResponse<DownloadLeadsFileDto?>> Handle(
		DownloadLeadsFileQueryRequest request,
        CancellationToken cancellationToken)
    {
        var leadsFile = await _leadRepository.GetLeadsFileByIdAsync(request.Id, cancellationToken);
        if (leadsFile is null)
		    return ApplicationResponse<DownloadLeadsFileDto?>.Create(
			    default,
			    operationCode: OperationCodes.NotFound);

		var file = await _fileStorageProvider.DownloadAsync(
            leadsFile.FileId,
			cancellationToken: cancellationToken);

        return ApplicationResponse<DownloadLeadsFileDto?>.Create(
			DownloadLeadsFileMapper.ToDto(file),
            operationCode: file is not null ? OperationCodes.Successful : OperationCodes.NotFound);
    }
}
