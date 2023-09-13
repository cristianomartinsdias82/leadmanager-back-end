using Application.Contracts.Caching;
using Application.Contracts.Persistence;
using Application.Features.Leads.Commands.UpdateLead;
using Application.Features.Leads.Shared;
using CrossCutting.MessageContracts;
using FluentAssertions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Shared.Events;
using Shared.Events.EventDispatching;
using Shared.Results;
using Shared.Tests;
using System.Threading;
using Tests.Common.ObjectMothers.Application;
using Tests.Common.ObjectMothers.Core;
using Xunit;

namespace Application.Tests.Leads.Commands.UpdateLead;

public sealed class UpdateLeadCommandRequestHandlerTests : IAsyncDisposable, IDisposable
{
    private readonly UpdateLeadCommandRequestHandler _handler;
    private readonly ILeadManagerDbContext _dbContext;
    private readonly ICachingManagement _cachingManager;
    private readonly IMediator _mediator;
    private readonly IEventDispatching _eventDispatcher;
    private readonly CancellationTokenSource _cts;    

    public UpdateLeadCommandRequestHandlerTests()
    {
        _dbContext = InMemoryLeadManagerDbContextFactory.Create();
        _mediator = Substitute.For<IMediator>();
        _eventDispatcher = Substitute.For<IEventDispatching>();
        _cachingManager = Substitute.For<ICachingManagement>();
        _handler = new UpdateLeadCommandRequestHandler(_mediator, _eventDispatcher, _dbContext, _cachingManager);
        _cts = new();
    }

    public void Dispose()
    {
        _cts.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        await _dbContext.Leads.ExecuteDeleteAsync();
    }

    [Fact]
    public async Task Handle_ValidRequestParametersAndExistingLead_ShouldSucceed()
    {
        //Arrange
        var leadId = Guid.NewGuid();
        var lead = LeadMother.XptoLLC();
        var updateRequest = UpdateLeadCommandRequestMother
                        .Instance
                        .WithId(leadId)
                        .WithRazaoSocial("XPTO Brasil LLC")
                        .WithCep("11111-111")
                        .WithEndereco("Avenida Carlos Ribeiro")
                        .WithCidade("Aracaju")
                        .WithEstado("SE")
                        .WithRowVersion(lead.RowVersion)
                        .Build();
        
        lead.Id = leadId;
        
        await _dbContext.Leads.AddAsync(lead, _cts.Token);
        await _dbContext.SaveChangesAsync(_cts.Token);

        //Act
        var result = await _handler.Handle(updateRequest, _cts.Token);

        //Assert
        result.Should().NotBeNull();
        result.OperationCode.Should().Be(OperationCodes.Successful);
        result.Success.Should().BeTrue();
        result.Inconsistencies.Should().BeNullOrEmpty();
        result.Should().BeOfType<ApplicationResponse<UpdateLeadCommandResponse>>();
        var updatedLead = await _dbContext.Leads.FindAsync(updateRequest.Id, _cts.Token);
        updatedLead.Should().NotBeNull();
        updatedLead!.Cep.Should().Be(updateRequest.Cep);
        updatedLead.Logradouro.Should().Be(updateRequest.Endereco);
        updatedLead.Cidade.Should().Be(updateRequest.Cidade);
        updatedLead.Estado.Should().Be(updateRequest.Estado);
        _eventDispatcher.Received(1).AddEvent(Arg.Any<IEvent>());
        await _cachingManager.Received(1).UpdateLeadEntryAsync(Arg.Any<LeadDto>(), _cts.Token);
    }

    [Fact]
    public async Task Handle_ValidRequestParametersWithNonExistingLead_ShouldReturnResultObjectWithNotFoundMessage()
    {
        //Arrange
        var updateRequest = UpdateLeadCommandRequestMother
                        .Instance
                        .WithId(Guid.NewGuid())
                        .WithRazaoSocial("XPTO Brasil LLC")
                        .WithCep("11111-111")
                        .WithEndereco("Avenida Carlos Ribeiro")
                        .WithCidade("Salto")
                        .WithEstado("SP")
                        .Build();

        await _dbContext.Leads.AddAsync(LeadMother.XptoLLC(), _cts.Token);
        await _dbContext.SaveChangesAsync(_cts.Token);

        //Act
        var result = await _handler.Handle(updateRequest, _cts.Token);

        //Assert
        result.Should().NotBeNull();
        result.OperationCode.Should().Be(OperationCodes.Successful);
        result.Success.Should().BeTrue();
        result.Inconsistencies.Should().BeNullOrEmpty();
        result.Should().BeOfType<ApplicationResponse<UpdateLeadCommandResponse>>();
        result.Message.Should().BeEquivalentTo("Lead não encontrado.");
        _eventDispatcher.Received(0).AddEvent(Arg.Any<IEvent>());
        await _cachingManager.Received(0).UpdateLeadEntryAsync(Arg.Any<LeadDto>(), _cts.Token);
    }

    //TODO: Implement Concurrency checking lead update operations
    ////This will only work if you consider using Testing Containers!
    ////https://www.youtube.com/watch?v=tj5ZCtvgXKY (The Best Way To Use Docker For Integration Testing In .NET)
    //[Fact]
    //public async Task Handle_UpdateSameDataConcurrently_ShouldFail()
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
    //public async Task Handle_UpdateDataFromConcurrentlyRemovedLeadSuccessfully_ShouldFail()
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
