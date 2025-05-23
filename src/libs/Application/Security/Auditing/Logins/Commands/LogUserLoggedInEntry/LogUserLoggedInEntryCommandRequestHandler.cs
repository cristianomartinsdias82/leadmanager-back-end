﻿using Application.Core;
using Application.Core.Contracts.Repository.Security.Auditing;
using Application.Core.Contracts.Repository.UnitOfWork;
using CrossCutting.Security.IAM;
using MediatR;
using Shared.Events.EventDispatching;
using Shared.FrameworkExtensions;
using Shared.RequestHandling;
using Shared.Results;

namespace Application.Security.Auditing.Logins.Commands.LogUserLoggedInEntry;

internal sealed class LogUserLoggedInEntryCommandRequestHandler : ApplicationRequestHandler<LogUserLoggedInEntryCommandRequest, LogUserLoggedInEntryCommandResponse>
{
    private readonly IUserService _userService;
    private readonly IAuditingRepository _auditingRepository;
    private readonly IUnitOfWork _unitOfWork;
	private readonly TimeProvider _timeProvider;

	public LogUserLoggedInEntryCommandRequestHandler(
        IMediator mediator,
        IEventDispatching eventDispatcher,
        IUserService userService,
        IAuditingRepository auditingRepository,
        IUnitOfWork unitOfWork,
        TimeProvider timeProvider) : base(mediator, eventDispatcher)
    {
        _userService = userService;
        _auditingRepository = auditingRepository;
        _unitOfWork = unitOfWork;
        _timeProvider = timeProvider;

	}

    public async override Task<ApplicationResponse<LogUserLoggedInEntryCommandResponse>> Handle(LogUserLoggedInEntryCommandRequest request, CancellationToken cancellationToken)
    {
        await Task.Factory.StartNew(async () =>
        {
            await _auditingRepository.AddAsync(AuditEntry.Create(
															_timeProvider.GetLocalNow(),
                                                            _userService.GetUserEmail()!,
                                                            SystemActions.Login,
                                                            null,
                                                            null,
                                                            EnumExtensions.GetEnumDescription(SystemActions.Login),
                                                            null),
                                                cancellationToken);

            await _unitOfWork.CommitAsync(cancellationToken);
        }, TaskCreationOptions.LongRunning);

        //AddEvent(new UserLoggedInIntegrationEvent(_userService.GetUserEmail()!));

        return ApplicationResponse<LogUserLoggedInEntryCommandResponse>
                .Create(new());
    }
}