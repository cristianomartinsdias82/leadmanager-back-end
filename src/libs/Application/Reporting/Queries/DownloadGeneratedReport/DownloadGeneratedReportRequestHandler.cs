using Application.Core.Contracts.Persistence;
using CrossCutting.FileStorage;
using CrossCutting.FileStorage.Configuration;
using CrossCutting.Security.IAM;
using MediatR;
using Shared.RequestHandling;
using Shared.Results;

namespace Application.Reporting.Queries.DownloadGeneratedReport;

internal sealed class DownloadGeneratedReportQueryRequestHandler
	: ApplicationRequestHandler<DownloadGeneratedReportQueryRequest, PersistableData?>
{
	private readonly IFileStorageProvider _fileStorageProvider;
	private readonly StorageProviderSettings _storageProviderSettings;
	private readonly ILeadManagerDbContext _dbContext;
	private readonly IUserService _userService;

	public DownloadGeneratedReportQueryRequestHandler(
        IMediator mediator,
		IFileStorageProvider fileStorageProvider,
		StorageProviderSettings storageProviderSettings,
		IUserService userService,
		ILeadManagerDbContext dbContext)
         : base(mediator, default!)
    {
		_fileStorageProvider = fileStorageProvider;
		_storageProviderSettings = storageProviderSettings;
		_userService = userService;
		_dbContext = dbContext;
	}

    public async override Task<ApplicationResponse<PersistableData?>> Handle(
		DownloadGeneratedReportQueryRequest request,
        CancellationToken cancellationToken = default)
    {
        var reportGenerationRequest = await _dbContext
                                                .ReportGenerationRequests
                                                .FindAsync(request.Id, cancellationToken);

        if (reportGenerationRequest is null || string.IsNullOrWhiteSpace(reportGenerationRequest.GeneratedFileName))
			return ApplicationResponse<PersistableData?>.Create(
			        default!,
			        operationCode: OperationCodes.NotFound);

		var file = await _fileStorageProvider.DownloadAsync(
			blobName: reportGenerationRequest.GeneratedFileName,
            blobPath: @$"{_storageProviderSettings.ReportGenerationStorageSettings.RootFolder}/{_userService.GetUserId()}",
            cancellationToken: cancellationToken);

        return ApplicationResponse<PersistableData?>.Create(
			    DownloadGeneratedReportMapper.ToDto(file));
    }
}
