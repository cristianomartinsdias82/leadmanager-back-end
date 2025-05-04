using Application.Core.Contracts.Repository.Prospecting;
using CrossCutting.FileStorage;
using CrossCutting.FileStorage.Configuration;
using MediatR;
using Shared.RequestHandling;
using Shared.Results;

namespace Application.Prospecting.Leads.Commands.BulkRemoveLeadsFiles;

internal sealed class BulkRemoveLeadsFilesCommandRequestHandler : ApplicationRequestHandler<BulkRemoveLeadsFilesCommandRequest, bool>
{
	private readonly ILeadRepository _leadRepository;
	private readonly IFileStorageProvider _fileStorageProvider;
	private readonly StorageProviderSettings _storageProviderSettings;

	public BulkRemoveLeadsFilesCommandRequestHandler(
		IMediator mediator,
		ILeadRepository leadRepository,
		IFileStorageProvider fileStorageProvider,
		StorageProviderSettings storageProviderSettings)
		 : base(mediator, default!)
	{
		_leadRepository = leadRepository;
		_fileStorageProvider = fileStorageProvider;
		_storageProviderSettings = storageProviderSettings;
	}

	public async override Task<ApplicationResponse<bool>> Handle(
		BulkRemoveLeadsFilesCommandRequest request,
		CancellationToken cancellationToken = default)
	{
		await Task.WhenAll([
			Task.Factory.StartNew(async () => await _leadRepository.RemoveLeadsFilesByIdsAsync([..request.Ids.Select(f => f.Id)], cancellationToken), cancellationToken),
			Task.Factory.StartNew(async () => await _fileStorageProvider.BatchRemoveAsync(
										[..request.Ids.Select(f => f.FileId)],
										_storageProviderSettings.ContainerName,
										cancellationToken), cancellationToken)
		]);

		return ApplicationResponse<bool>.Create(true);
	}
}
