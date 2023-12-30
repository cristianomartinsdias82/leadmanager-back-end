using Application.Core.Contracts.Repository.Prospecting;
using Application.Prospecting.Leads.Commands.RegisterLead;
using CrossCutting.Security.IAM;
using Domain.Prospecting.Entities;
using FluentAssertions;
using MediatR;
using NSubstitute;
using Shared.Events;
using Shared.Events.EventDispatching;
using Shared.Results;
using Tests.Common.ObjectMothers.Application;
using Xunit;

namespace Application.Tests.Prospecting.Leads.Commands.RegisterLead;

public sealed class RegisterLeadCommandRequestHandlerTests
{
    private readonly RegisterLeadCommandRequestHandler _handler;
    private readonly IUserService _userService;
    private readonly ILeadRepository _leadRepository;
    private readonly IMediator _mediator;
    private readonly IEventDispatching _eventDispatcher;
    private readonly CancellationTokenSource _cts;

    public RegisterLeadCommandRequestHandlerTests()
    {
        _userService = Substitute.For<IUserService>();
        _userService.GetUserId().Returns(Guid.NewGuid());
        _mediator = Substitute.For<IMediator>();
        _leadRepository = Substitute.For<ILeadRepository>();
        _eventDispatcher = Substitute.For<IEventDispatching>();
        _handler = new RegisterLeadCommandRequestHandler(_mediator, _eventDispatcher, _leadRepository);
        _cts = new();
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
        await _leadRepository.Received(1).AddAsync(Arg.Any<Lead>(), Arg.Any<CancellationToken>());
        _eventDispatcher.Received(1).AddEvent(Arg.Any<IEvent>());
    }
}
