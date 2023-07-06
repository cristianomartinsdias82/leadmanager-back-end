using Application.Contracts.Persistence;
using Application.Features.Leads.Commands.RegisterLead;
using Application.Tests.Utils.Factories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Shared.Results;
using Tests.Common.ObjectMothers.Application;
using Xunit;

namespace Application.Tests.Leads.Commands.RegisterLead;

public sealed class RegisterLeadCommandHandlerTests : IAsyncDisposable
{
    private readonly ILeadManagerDbContext _dbContext;

    public RegisterLeadCommandHandlerTests()
    {
        _dbContext = InMemoryLeadManagerDbContextFactory.Create();
    }

    public async ValueTask DisposeAsync()
    {
        await _dbContext.Leads.ExecuteDeleteAsync();
    }

    [Fact]
    public async Task Handle_ValidRequestParameters_ShouldSucceed()
    {
        //Arrange
        var handler = new RegisterLeadCommandHandler(_dbContext);
        using var cts = new CancellationTokenSource();
        var request = RegisterLeadCommandRequestMother.XptoIncLeadRequest();

        //Act
        var result = await handler.Handle(request, cts.Token);

        //Assert
        result.Should().NotBeNull();
        result.OperationCode.Should().Be(OperationCodes.Successful);
        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Inconsistencies.Should().BeNullOrEmpty();
        result.Should().BeOfType<ApplicationResponse<RegisterLeadCommandResponse>>();
        result.Data.Id.Should().NotBe(Guid.Empty);
        var newlyCreatedLead = await _dbContext.Leads.FindAsync(result.Data.Id, cts.Token);
        newlyCreatedLead.Should().NotBeNull();
        newlyCreatedLead!.Cnpj.Should().BeEquivalentTo(request.Cnpj);
    }
}
