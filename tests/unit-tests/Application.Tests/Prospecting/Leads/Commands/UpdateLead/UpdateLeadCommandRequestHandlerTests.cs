using Application.Core.Contracts.Repository.Prospecting;
using Application.Prospecting.Leads.Commands.UpdateLead;
using Domain.Prospecting.Entities;
using FluentAssertions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Shared.Events;
using Shared.Events.EventDispatching;
using Shared.Results;
using Tests.Common.ObjectMothers.Application;
using Tests.Common.ObjectMothers.Domain;
using Xunit;

namespace Application.Tests.Prospecting.Leads.Commands.UpdateLead;

public sealed class UpdateLeadCommandRequestHandlerTests
{
    private readonly UpdateLeadCommandRequestHandler _handler;
    private readonly IMediator _mediator;
    private readonly ILeadRepository _leadRepository;
    private readonly IEventDispatching _eventDispatcher;
    private readonly CancellationTokenSource _cts;

    public UpdateLeadCommandRequestHandlerTests()
    {
        _mediator = Substitute.For<IMediator>();
        _leadRepository = Substitute.For<ILeadRepository>();
        _eventDispatcher = Substitute.For<IEventDispatching>();
        _handler = new UpdateLeadCommandRequestHandler(_mediator, _eventDispatcher, _leadRepository);
        _cts = new();
    }

    [Fact]
    public async Task Handle_ConcurrentUpdateRequestsOverSameRecord_ShouldSignalConcurrencyIssue()
    {
        //Arrange
        var lead = LeadMother.XptoLLC();
        var request = UpdateLeadCommandRequestMother.XptoIncLeadRequest();

        _leadRepository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
                       .Returns(lead);

        _leadRepository.UpdateAsync(lead, lead.RowVersion, Arg.Any<CancellationToken>())
                       .ReturnsForAnyArgs(
                            /*On first execution, return*/ Task.CompletedTask,
                            /*On second execution, return*/ Task.FromException<DbUpdateConcurrencyException>(new DbUpdateConcurrencyException())
                       );

        //Act
        var firstRequest = await _handler.Handle(request, _cts.Token);
        var secondRequest = await _handler.Handle(request, _cts.Token);

        //Assert
        firstRequest.Success.Should().BeTrue();
        secondRequest.Success.Should().BeFalse();
        secondRequest.Exception.Should().NotBeNull();
        secondRequest.Exception!.ExceptionType.Should().BeEquivalentTo(typeof(DbUpdateConcurrencyException).FullName);
        secondRequest.OperationCode.Should().Be(OperationCodes.ConcurrencyIssue);
        await _leadRepository.Received(2).UpdateAsync(Arg.Any<Lead>(), Arg.Any<byte[]>(), Arg.Any<CancellationToken>());
        _eventDispatcher.Received(1).AddEvent(Arg.Any<IEvent>());
    }

    [Fact]
    public async Task Handle_ValidRequestParametersAndExistingLead_ShouldSucceed()
    {
        //Arrange
        var lead = LeadMother.XptoLLC();
        var updateRequest = UpdateLeadCommandRequestMother.XptoIncLeadRequest();

        _leadRepository.GetByIdAsync(Arg.Any<Guid>(), _cts.Token).Returns(lead);
        _leadRepository.UpdateAsync(lead, lead.RowVersion, _cts.Token).Returns(Task.CompletedTask);

        //Act
        var result = await _handler.Handle(updateRequest, _cts.Token);

        //Assert
        result.Should().NotBeNull();
        result.OperationCode.Should().Be(OperationCodes.Successful);
        result.Success.Should().BeTrue();
        result.Inconsistencies.Should().BeNullOrEmpty();
        result.Should().BeOfType<ApplicationResponse<UpdateLeadCommandResponse>>();
        await _leadRepository.Received(1).GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
        await _leadRepository.Received(1).UpdateAsync(Arg.Any<Lead>(), Arg.Any<byte[]>(), Arg.Any<CancellationToken>());
        _eventDispatcher.Received(1).AddEvent(Arg.Any<IEvent>());
    }

    [Fact]
    public async Task Handle_ValidRequestParametersWithNonExistingLead_ShouldReturnResultObjectWithNotFoundMessage()
    {
        //Arrange
        var updateRequest = UpdateLeadCommandRequestMother.XptoIncLeadRequest();

        _leadRepository.GetByIdAsync(Guid.NewGuid(), _cts.Token).Returns(default(Lead));

        //Act
        var result = await _handler.Handle(updateRequest, _cts.Token);

        //Assert
        result.Should().NotBeNull();
        result.OperationCode.Should().Be(OperationCodes.Successful);
        result.Success.Should().BeTrue();
        result.Inconsistencies.Should().BeNullOrEmpty();
        result.Should().BeOfType<ApplicationResponse<UpdateLeadCommandResponse>>();
        result.Message.Should().BeEquivalentTo("Lead não encontrado.");
        await _leadRepository.Received(1).GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
        await _leadRepository.Received(0).UpdateAsync(Arg.Any<Lead>(), Arg.Any<byte[]>(), Arg.Any<CancellationToken>());
        _eventDispatcher.Received(0).AddEvent(Arg.Any<IEvent>());
    }
}
