using Application.Core.Contracts.Repository.Prospecting;
using Application.Prospecting.Leads.Commands.RemoveLead;
using Domain.Prospecting.Entities;
using FluentAssertions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Shared.Events;
using Shared.Events.EventDispatching;
using Shared.Results;
using Tests.Common.ObjectMothers.Domain;
using Xunit;

namespace Application.Tests.Prospecting.Leads.Commands.RemoveLead;

public sealed class RemoveLeadCommandRequestHandlerTests
{
    private readonly RemoveLeadCommandRequestHandler _handler;
    private readonly IMediator _mediator;
    private readonly ILeadRepository _leadRepository;
    private readonly IEventDispatching _eventDispatcher;
    private readonly CancellationTokenSource _cts;

    public RemoveLeadCommandRequestHandlerTests()
    {
        _mediator = Substitute.For<IMediator>();
        _leadRepository = Substitute.For<ILeadRepository>();
        _eventDispatcher = Substitute.For<IEventDispatching>();
        _handler = new RemoveLeadCommandRequestHandler(_mediator, _leadRepository, _eventDispatcher);
        _cts = new();
    }

    [Fact]
    public async Task Handle_ConcurrentDeleteRequestsOverSameRecord_ShouldSignalConcurrencyIssue()
    {
        //Arrange
        var lead = LeadMother.XptoLLC();
        var request = new RemoveLeadCommandRequest
        {
            Id = lead.Id,
            Revision = lead.RowVersion
        };

        _leadRepository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
                       .Returns(lead);

        _leadRepository.RemoveAsync(lead, lead.RowVersion, Arg.Any<CancellationToken>())
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
        await _leadRepository.Received(2).RemoveAsync(Arg.Any<Lead>(), Arg.Any<byte[]>(), Arg.Any<CancellationToken>());
        _eventDispatcher.Received(1).AddEvent(Arg.Any<IEvent>());
    }

    [Fact]
    public async Task Handle_WithExistingLead_ShouldReturnResultObject()
    {
        //Arrange
        var lead = LeadMother.XptoLLC();
        var deleteRequest = new RemoveLeadCommandRequest { Id = lead.Id };
        _leadRepository.GetByIdAsync(Arg.Any<Guid>(), _cts.Token).ReturnsForAnyArgs(lead);

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
        await _leadRepository.Received(1).GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
        await _leadRepository.Received(1).RemoveAsync(Arg.Any<Lead>(), Arg.Any<byte[]>(), Arg.Any<CancellationToken>());
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
        result.Message.Should().BeEquivalentTo("Lead não encontrado.");
        await _leadRepository.Received(1).GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
        await _leadRepository.Received(0).RemoveAsync(Arg.Any<Lead>(), Arg.Any<byte[]>(), Arg.Any<CancellationToken>());
        _eventDispatcher.Received(0).AddEvent(Arg.Any<IEvent>());
    }
}
