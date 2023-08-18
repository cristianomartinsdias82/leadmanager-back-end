using Application.Contracts.Persistence;
using Application.Features.Leads.Commands.RemoveLead;
using FluentAssertions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Shared.Events.EventDispatching;
using Shared.Results;
using Shared.Tests;
using Tests.Common.ObjectMothers.Core;
using Xunit;

namespace Application.Tests.Leads.Commands.RemoveLead;

public sealed class RemoveLeadCommandHandlerTests : IAsyncDisposable, IDisposable
{
    private readonly RemoveLeadCommandHandler _handler;
    private readonly ILeadManagerDbContext _dbContext;
    private readonly IMediator _mediator;
    private readonly IEventDispatching _eventDispatcher;
    private readonly CancellationTokenSource _cts;

    public RemoveLeadCommandHandlerTests()
    {
        _dbContext = InMemoryLeadManagerDbContextFactory.Create();
        _mediator = Substitute.For<IMediator>();
        _eventDispatcher = Substitute.For<IEventDispatching>();
        _handler = new RemoveLeadCommandHandler(_mediator, _eventDispatcher, _dbContext);
        _cts = new();
    }

    public async ValueTask DisposeAsync()
    {
        await _dbContext.Leads.ExecuteDeleteAsync();
    }

    public void Dispose()
    {
        _cts.Dispose();
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
    }

    [Fact]
    public async Task Handle_WithExistingLead_ShouldReturnResultObject()
    {
        //Arrange
        var leadToRemove = LeadMother.XptoLLC();
        await _dbContext.Leads.AddAsync(leadToRemove);
        await _dbContext.SaveChangesAsync(_cts.Token);
        var request = new RemoveLeadCommandRequest { Id = leadToRemove.Id };

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
    }
}
