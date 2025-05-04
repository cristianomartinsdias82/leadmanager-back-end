using Application.Core.Contracts.Repository.Prospecting;
using Application.Core.Contracts.Repository.UnitOfWork;
using Application.Prospecting.Leads.Commands.RemoveLead;
using Domain.Prospecting.Entities;
using FluentAssertions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Shared.Events;
using Shared.Events.EventDispatching;
using Shared.Results;
using Tests.Common.ObjectMothers.Domain;
using Xunit;

namespace Application.Tests.Prospecting.Leads.Commands.RemoveLead;

public sealed class RemoveLeadCommandRequestHandlerTests
{
	private const string LeadNotFound = "Lead não encontrado.";
	private readonly RemoveLeadCommandRequestHandler _handler;
    private readonly IMediator _mediator;
    private readonly ILeadRepository _leadRepository;
    private readonly IEventDispatching _eventDispatcher;
	private readonly IUnitOfWork _unitOfWork;
	private readonly CancellationTokenSource _cts;

    public RemoveLeadCommandRequestHandlerTests()
    {
        _mediator = Substitute.For<IMediator>();
        _leadRepository = Substitute.For<ILeadRepository>();
        _eventDispatcher = Substitute.For<IEventDispatching>();
		_unitOfWork = Substitute.For<IUnitOfWork>();
		_handler = new RemoveLeadCommandRequestHandler(_mediator, _eventDispatcher, _leadRepository, _unitOfWork);
        _cts = new();
    }

	[Fact]
	public async Task Handle_ConcurrentDeleteRequestsOverSameRecord_ShouldThrowExceptionAndReturnConcurrencyIssue()
	{
		//Arrange
		var lead = LeadMother.XptoLLC();
		var request = new RemoveLeadCommandRequest
		{
			Id = lead.Id,
			Revision = lead.RowVersion
		};

		_leadRepository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<bool>(), Arg.Any<CancellationToken>())
					   .Returns(lead);

		_unitOfWork.CommitAsync(Arg.Any<CancellationToken>())
					   .Returns(Task.FromResult(1));

		//Act
		var firstRequest = await _handler.Handle(request, _cts.Token);

		//Small rearrangement
		_unitOfWork.CommitAsync(Arg.Any<CancellationToken>())
					   .Throws<DbUpdateConcurrencyException>();

        //Act
		var secondRequest = await _handler.Handle(request, _cts.Token);

		//Assert
		firstRequest.Success.Should().BeTrue();
		secondRequest.Success.Should().BeFalse();
		secondRequest.Exception.Should().NotBeNull();
		secondRequest.Exception!.ExceptionType.Should().BeEquivalentTo(typeof(DbUpdateConcurrencyException).FullName);
		secondRequest.OperationCode.Should().Be(OperationCodes.ConcurrencyIssue);
		await _leadRepository.Received(2).RemoveAsync(Arg.Any<Lead>(), Arg.Any<CancellationToken>());
		await _unitOfWork.Received(2).CommitAsync(Arg.Any<CancellationToken>());
		_eventDispatcher.Received(1).AddEvent(Arg.Any<IEvent>());
	}

	[Fact]
    public async Task Handle_WithExistingLead_ShouldReturnResultObject()
    {
        //Arrange
        var lead = LeadMother.XptoLLC();
        var deleteRequest = new RemoveLeadCommandRequest { Id = lead.Id };
        _leadRepository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<bool>(), _cts.Token).ReturnsForAnyArgs(lead);

        //Act
        var result = await _handler.Handle(deleteRequest, _cts.Token);

        //Assert
        result.Should().NotBeNull();
        result.OperationCode.Should().Be(OperationCodes.Successful);
        result.Success.Should().BeTrue();
        result.Should().BeOfType<ApplicationResponse<RemoveLeadCommandResponse>>();
        result.Data.Should().NotBeNull();
        result.Inconsistencies.Should().BeNullOrEmpty();
        result.Message.Should().BeNullOrEmpty();
        await _leadRepository.Received(1).GetByIdAsync(Arg.Any<Guid>(), Arg.Any<bool>(), Arg.Any<CancellationToken>());
        await _leadRepository.Received(1).RemoveAsync(Arg.Any<Lead>(), Arg.Any<CancellationToken>());
        _eventDispatcher.Received(1).AddEvent(Arg.Any<IEvent>());
    }

    [Fact]
    public async Task Handle_WithNonExistingLead_ShouldReturnResultObjectWithNotFoundMessage()
    {
        //Arrange
        var request = new RemoveLeadCommandRequest { Id = Guid.NewGuid() };

        //Act
        var result = await _handler.Handle(request, _cts.Token);

        //Assert
        result.Should().NotBeNull();
        result.OperationCode.Should().Be(OperationCodes.Successful);
        result.Success.Should().BeTrue();
        result.Should().BeOfType<ApplicationResponse<RemoveLeadCommandResponse>>();
        result.Data.Should().BeNull();
        result.Inconsistencies.Should().BeNullOrEmpty();
        result.Message.Should().BeEquivalentTo(LeadNotFound);
        await _leadRepository.Received(1).GetByIdAsync(Arg.Any<Guid>(), Arg.Any<bool>(), Arg.Any<CancellationToken>());
        await _leadRepository.Received(0).RemoveAsync(Arg.Any<Lead>(), Arg.Any<CancellationToken>());
        _eventDispatcher.Received(0).AddEvent(Arg.Any<IEvent>());
    }
}
