using Application.Contracts.Persistence;
using Application.Features.Leads.Commands.RegisterLead;
using Application.Tests.Utils.Factories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Shared.Results;
using Tests.Common.ObjectMothers.Application;
using Xunit;

namespace Application.Tests.Leads.Commands.RegisterLead;

public sealed class RegisterLeadCommandHandlerTests : IAsyncDisposable, IDisposable
{
    private readonly ILeadManagerDbContext _dbContext;
    private readonly CancellationTokenSource _cts;

    public RegisterLeadCommandHandlerTests()
    {
        _dbContext = InMemoryLeadManagerDbContextFactory.Create();
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
        var handler = new RegisterLeadCommandHandler(_dbContext);
        var request = RegisterLeadCommandRequestMother.XptoIncLeadRequest();

        //Act
        var result = await handler.Handle(request, _cts.Token);

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
    }
}
