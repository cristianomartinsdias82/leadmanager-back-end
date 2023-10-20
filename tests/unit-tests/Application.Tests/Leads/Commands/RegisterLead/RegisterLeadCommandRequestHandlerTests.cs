using Application.Contracts.Caching;
using Application.Contracts.Persistence;
using Application.Features.Leads.Commands.RegisterLead;
using Application.Features.Leads.Shared;
using CrossCutting.Security.IAM;
using FluentAssertions;
using MediatR;
using NSubstitute;
using Shared.Events;
using Shared.Events.EventDispatching;
using Shared.Results;
using Shared.Tests;
using Tests.Common.ObjectMothers.Application;
using Xunit;

namespace Application.Tests.Leads.Commands.RegisterLead;

public sealed class RegisterLeadCommandRequestHandlerTests : IAsyncDisposable
{
    private readonly RegisterLeadCommandRequestHandler _handler;
    private readonly IUserService _userService;
    private readonly ILeadManagerDbContext _dbContext;
    private readonly ICachingManagement _cachingManager;
    private readonly IMediator _mediator;
    private readonly IEventDispatching _eventDispatcher;
    private readonly CancellationTokenSource _cts;

    public RegisterLeadCommandRequestHandlerTests()
    {
        _userService = Substitute.For<IUserService>();
        _userService.GetUserId().Returns(Guid.NewGuid());
        _dbContext = InMemoryLeadManagerDbContextFactory.Create(_userService);
        _mediator = Substitute.For<IMediator>();
        _eventDispatcher = Substitute.For<IEventDispatching>();
        _cachingManager = Substitute.For<ICachingManagement>();
        _handler = new RegisterLeadCommandRequestHandler(_mediator, _eventDispatcher, _dbContext, _cachingManager);
        _cts = new();
    }

    public async ValueTask DisposeAsync()
    {
        _cts.Dispose();
        await _dbContext.DisposeAsync();
    }

    [Fact]
    public async Task Handle_ValidRequestParameters_ShouldSucceed()
    {
        //Arrange
        var request = RegisterLeadCommandRequestMother.XptoIncLeadRequest();

        //Act
        var result = await _handler.Handle(request, _cts.Token);

        //Assert
        result.Should().NotBeNull();
        result.OperationCode.Should().Be(OperationCodes.Successful);
        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Inconsistencies.Should().BeNullOrEmpty();
        result.Should().BeOfType<ApplicationResponse<RegisterLeadCommandResponse>>();
        result.Data.Id.Should().NotBe(Guid.Empty);
        var newlyCreatedLead = await _dbContext.Leads.FindAsync(result.Data.Id, _cts.Token);
        newlyCreatedLead.Should().NotBeNull();
        newlyCreatedLead!.Cnpj.Should().BeEquivalentTo(request.Cnpj);
        _eventDispatcher.Received(1).AddEvent(Arg.Any<IEvent>());
        await _cachingManager.Received(1).AddLeadEntryAsync(Arg.Any<LeadDto>(), _cts.Token);
    }
}
