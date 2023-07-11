using Application.Contracts.Persistence;
using Application.Features.Leads.Commands.RemoveLead;
using Application.Tests.Utils.Factories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Shared.Results;
using Tests.Common.ObjectMothers.Core;
using Xunit;

namespace Application.Tests.Leads.Commands.RemoveLead;

public sealed class RemoveLeadCommandHandlerTests : IAsyncDisposable
{
    private readonly ILeadManagerDbContext _dbContext;

    public RemoveLeadCommandHandlerTests()
    {
        _dbContext = InMemoryLeadManagerDbContextFactory.Create();
    }

    public async ValueTask DisposeAsync()
    {
        await _dbContext.Leads.ExecuteDeleteAsync();
    }

    [Fact]
    public async Task Handle_WithNonExistingLead_ShouldReturnResultObjectWithNotFoundMessage()
    {
        //Arrange
        var handler = new RemoveLeadCommandHandler(_dbContext);
        using var cts = new CancellationTokenSource();
        var request = new RemoveLeadCommandRequest { Id = Guid.Empty };

        //Act
        var result = await handler.Handle(request, cts.Token);

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
        using var cts = new CancellationTokenSource();
        var leadToRemove = LeadMother.XptoLLC();
        await _dbContext.Leads.AddAsync(leadToRemove);
        await _dbContext.SaveChangesAsync(cts.Token);
        var handler = new RemoveLeadCommandHandler(_dbContext);
        var request = new RemoveLeadCommandRequest { Id = leadToRemove.Id };

        //Act
        var result = await handler.Handle(request, cts.Token);

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
