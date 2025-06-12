using Application.Core.Contracts.Persistence;
using CrossCutting.FileStorage;
using CrossCutting.FileStorage.Configuration;
using CrossCutting.Security.IAM;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Events.EventDispatching;
using Shared.RequestHandling;
using Shared.Results;

namespace Application.Reporting.Commands.RemoveRequestedReport;

internal sealed class RemoveRequestedReportCommandRequestHandler : ApplicationRequestHandler<RemoveRequestedReportCommandRequest, RemoveRequestedReportCommandResponse>
{
	private readonly IFileStorageProvider _fileStorageProvider;
	private readonly StorageProviderSettings _storageProviderSettings;
	private readonly ILeadManagerDbContext _dbContext;
	private readonly IUserService _userService;

	public RemoveRequestedReportCommandRequestHandler(
		IMediator mediator,
		IEventDispatching eventDispatcher,
		IFileStorageProvider fileStorageProvider,
		StorageProviderSettings storageProviderSettings,
		ILeadManagerDbContext dbContext,
		IUserService userService) : base(mediator, eventDispatcher)
	{
		_fileStorageProvider = fileStorageProvider;
		_storageProviderSettings = storageProviderSettings;
		_dbContext = dbContext;
		_userService = userService;
	}

	public async override Task<ApplicationResponse<RemoveRequestedReportCommandResponse>> Handle(
		RemoveRequestedReportCommandRequest request,
		CancellationToken cancellationToken = default)
	{
		var userId = _userService.GetUserId().ToString();
		var reportGenerationRequest = await _dbContext
												.ReportGenerationRequests
												.AsNoTracking()
												.FirstOrDefaultAsync(
													it => it.Id == request.Id && it.UserId == userId,
													cancellationToken);

		if (reportGenerationRequest is null || string.IsNullOrWhiteSpace(reportGenerationRequest.GeneratedFileName))
			return ApplicationResponse<RemoveRequestedReportCommandResponse>
				.Create(
					default!,
					operationCode: OperationCodes.NotFound
				);

		await _fileStorageProvider.BatchRemoveAsync(
									[@$"{_storageProviderSettings.ReportGenerationStorageSettings.RootFolder}/{userId}/{reportGenerationRequest.GeneratedFileName}"],
									cancellationToken: cancellationToken);

		return ApplicationResponse<RemoveRequestedReportCommandResponse>
				.Create(new RemoveRequestedReportCommandResponse());
	}
}
