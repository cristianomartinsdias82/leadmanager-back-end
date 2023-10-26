﻿using Application.Core.Contracts.Repository;
using Application.Prospecting.Leads.Commands.RemoveLead;
using Domain.Prospecting.Entities;
using FluentAssertions;
using MediatR;
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
    private readonly ILeadRepository _leadRepository;
    private readonly IMediator _mediator;
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

    //TODO: Implement Concurrency checking lead remove operations
    ////This will only work if you consider using Testing Containers!
    ////https://www.youtube.com/watch?v=tj5ZCtvgXKY (The Best Way To Use Docker For Integration Testing In .NET)
    //[Fact]
    //public async Task Handle_RemoveDataConcurrently_ShouldReturnSuccess()
    //{
    //    //Arrange
    //    var lead = LeadMother.XptoLLC();
    //    var initialRowVersion = lead.RowVersion;
    //    await _dbContext.Leads.AddAsync(lead, _cts.Token);
    //    await _dbContext.SaveChangesAsync(_cts.Token);

    //    var succeedingUpdateRequest = UpdateLeadCommandRequestMother
    //                    .Instance
    //                    .WithRowVersion(initialRowVersion)
    //                    .WithId(lead.Id)
    //                    .WithRazaoSocial("XPTO Brasil LLC")
    //                    .WithCep("84654-003")
    //                    .WithEndereco("Avenida Carlos Ribeiro")
    //                    .WithBairro("Vila Alexandria")
    //                    .WithCidade("Pindamonhangaba")
    //                    .WithEstado("SP")
    //                    .Build();

    //    var failingUpdateRequest = UpdateLeadCommandRequestMother
    //                    .Instance
    //                    .WithRowVersion(initialRowVersion)
    //                    .WithId(lead.Id)
    //                    .WithRazaoSocial("Rebel Yell Inc.")
    //                    .WithCep("04858-042")
    //                    .WithEndereco("Rua Constelação de Escorpião")
    //                    .WithBairro("Vila Alexandria")
    //                    .WithCidade("São Paulo")
    //                    .WithEstado("SP")
    //                    .Build();

    //    //Act
    //    await _handler.Handle(succeedingUpdateRequest, _cts.Token);
    //    var failingUpdateResult = await _handler.Handle(failingUpdateRequest, _cts.Token);

    //    //Assert
    //    failingUpdateResult.Should().NotBeNull();
    //    failingUpdateResult.Should().BeOfType<ApplicationResponse<UpdateLeadCommandResponse>>();
    //    failingUpdateResult.Success.Should().BeFalse();
    //    failingUpdateResult.OperationCode.Should().Be(OperationCodes.ConcurrencyIssue);
    //    failingUpdateResult.Data.Should().NotBeNull();
    //    failingUpdateResult.Data.RecordState.Should().Be(RecordStates.Modified);
    //    failingUpdateResult.Message.Should().NotBeNullOrWhiteSpace().And.BeEquivalentTo("Este registro foi atualizado por outro usuário antes desta operação de atualização.");

    //    _eventDispatcher.Received(0).AddEvent(Arg.Any<IEvent>());
    //    await _cachingManager.Received(0).UpdateLeadEntryAsync(Arg.Any<LeadDto>(), _cts.Token);
    //}

    ////This will only work if you consider using Testing Containers!
    ////https://www.youtube.com/watch?v=tj5ZCtvgXKY (The Best Way To Use Docker For Integration Testing In .NET)
    //[Fact]
    //public async Task Handle_RemoveDataFromConcurrentlyUpdatedLeadDataSuccessfully_ShouldFail()
    //{
    //    //Arrange
    //    var lead = LeadMother.XptoLLC();
    //    var failingUpdateRequest = UpdateLeadCommandRequestMother
    //                    .Instance
    //                    .WithRowVersion(lead.RowVersion)
    //                    .WithId(lead.Id)
    //                    .WithRazaoSocial("Rebel Yell Inc.")
    //                    .WithCep("04858-042")
    //                    .WithEndereco("Rua Constelação de Escorpião")
    //                    .WithBairro("Vila Alexandria")
    //                    .WithCidade("São Paulo")
    //                    .WithEstado("SP")
    //                    .Build();

    //    //Act
    //    await _handler.Handle(failingUpdateRequest, _cts.Token);
    //    var failingUpdateResult = await _handler.Handle(failingUpdateRequest, _cts.Token);

    //    //Assert
    //    failingUpdateResult.Should().NotBeNull();
    //    failingUpdateResult.Should().BeOfType<ApplicationResponse<UpdateLeadCommandResponse>>();
    //    failingUpdateResult.Success.Should().BeFalse();
    //    failingUpdateResult.OperationCode.Should().Be(OperationCodes.ConcurrencyIssue);
    //    failingUpdateResult.Data.Should().NotBeNull();
    //    failingUpdateResult.Data.RecordState.Should().Be(RecordStates.Deleted);
    //    failingUpdateResult.Message.Should().NotBeNullOrWhiteSpace().And.BeEquivalentTo("Este registro foi removido por outro usuário antes desta operação de atualização.");

    //    _eventDispatcher.Received(0).AddEvent(Arg.Any<IEvent>());
    //    await _cachingManager.Received(0).UpdateLeadEntryAsync(Arg.Any<LeadDto>(), _cts.Token);
    //}
}
