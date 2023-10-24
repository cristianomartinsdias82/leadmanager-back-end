using Application.Core.Contracts.Caching;
using Application.Core.Contracts.Persistence;
using Application.Prospecting.Leads.Commands.RemoveLead;
using Application.Prospecting.Leads.Shared;
using CrossCutting.Security.IAM;
using FluentAssertions;
using MediatR;
using NSubstitute;
using Shared.Events;
using Shared.Events.EventDispatching;
using Shared.Results;
using Tests.Common.Factories;
using Tests.Common.ObjectMothers.Domain;
using Xunit;

namespace Application.Tests.Prospecting.Leads.Commands.RemoveLead;

public sealed class RemoveLeadCommandRequestHandlerTests : IAsyncDisposable
{
    private readonly RemoveLeadCommandRequestHandler _handler;
    private readonly IUserService _userService;
    private readonly ILeadManagerDbContext _dbContext;
    private readonly ICachingManagement _cachingManager;
    private readonly IMediator _mediator;
    private readonly IEventDispatching _eventDispatcher;
    private readonly CancellationTokenSource _cts;

    public RemoveLeadCommandRequestHandlerTests()
    {
        _userService = Substitute.For<IUserService>();
        _userService.GetUserId().Returns(Guid.NewGuid());
        _dbContext = InMemoryLeadManagerDbContextFactory.Create(_userService);
        _mediator = Substitute.For<IMediator>();
        _eventDispatcher = Substitute.For<IEventDispatching>();
        _cachingManager = Substitute.For<ICachingManagement>();
        _handler = new RemoveLeadCommandRequestHandler(_mediator, _eventDispatcher, _dbContext, _cachingManager);
        _cts = new();
    }

    public async ValueTask DisposeAsync()
    {
        _cts.Dispose();
        await _dbContext.DisposeAsync();
    }

    [Fact]
    public async Task Handle_WithExistingLead_ShouldReturnResultObject()
    {
        //Arrange
        var leadToRemove = LeadMother.XptoLLC();
        await _dbContext.Leads.AddAsync(leadToRemove);
        await _dbContext.SaveChangesAsync(_cts.Token);
        var request = new RemoveLeadCommandRequest { Id = leadToRemove.Id, Revision = leadToRemove.RowVersion };

        //Act
        var result = await _handler.Handle(request, _cts.Token);

        //Assert
        result.Should().NotBeNull();
        result.OperationCode.Should().Be(OperationCodes.Successful);
        result.Success.Should().BeTrue();
        result.Should().BeOfType<ApplicationResponse<RemoveLeadCommandResponse>>();
        result.Data.Should().NotBeNull();
        result.Inconsistencies.Should().BeNullOrEmpty();
        result.Message.Should().BeNullOrEmpty();
        var lead = await _dbContext.Leads.FindAsync(request.Id);
        lead.Should().BeNull();
        _eventDispatcher.Received(1).AddEvent(Arg.Any<IEvent>());
        await _cachingManager.Received(1).RemoveLeadEntryAsync(Arg.Any<LeadDto>(), _cts.Token);
    }

    [Fact]
    public async Task Handle_WithNonExistingLead_ShouldReturnResultObjectWithNotFoundMessage()
    {
        //Arrange
        var request = new RemoveLeadCommandRequest { Id = Guid.Empty };

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
        _eventDispatcher.Received(0).AddEvent(Arg.Any<IEvent>());
        await _cachingManager.Received(0).RemoveLeadEntryAsync(Arg.Any<LeadDto>(), _cts.Token);
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
