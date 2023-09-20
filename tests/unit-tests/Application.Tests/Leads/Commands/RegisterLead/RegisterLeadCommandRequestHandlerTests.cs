﻿using Application.Contracts.Caching;
using Application.Contracts.Persistence;
using Application.Features.Leads.Commands.RegisterLead;
using Application.Features.Leads.Shared;
using FluentAssertions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Shared.Events;
using Shared.Events.EventDispatching;
using Shared.Results;
using Shared.Tests;
using Tests.Common.ObjectMothers.Application;
using Xunit;

namespace Application.Tests.Leads.Commands.RegisterLead;

public sealed class RegisterLeadCommandRequestHandlerTests : IAsyncDisposable, IDisposable
{
    private readonly RegisterLeadCommandRequestHandler _handler;
    private readonly ILeadManagerDbContext _dbContext;
    private readonly ICachingManagement _cachingManager;
    private readonly IMediator _mediator;
    private readonly IEventDispatching _eventDispatcher;
    private readonly CancellationTokenSource _cts;

    public RegisterLeadCommandRequestHandlerTests()
    {
        _dbContext = InMemoryLeadManagerDbContextFactory.Create();
        _mediator = Substitute.For<IMediator>();
        _eventDispatcher = Substitute.For<IEventDispatching>();
        _cachingManager = Substitute.For<ICachingManagement>();
        _handler = new RegisterLeadCommandRequestHandler(_mediator, _eventDispatcher, _dbContext, _cachingManager);
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